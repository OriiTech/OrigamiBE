using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Auth;
using Origami.BusinessTier.Payload.User;
using Origami.BusinessTier.Utils;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Npgsql;

namespace Origami.API.Services.Implement
{
    public class AuthService : BaseService<AuthService>, IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<AuthService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Email.ToLower() == request.Email.ToLower(),
                asNoTracking: true
            );

            if (existingUser != null)
                throw new BadHttpRequestException("EmailAlreadyExists");

            // Kiểm tra username đã tồn tại chưa (nếu cần)
            var existingUsername = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Username.ToLower() == request.Username.ToLower(),
                asNoTracking: true
            );

            if (existingUsername != null)
                throw new BadHttpRequestException("UsernameAlreadyExists");

            // Kiểm tra RoleId có tồn tại không (nếu có)
            if (request.RoleId.HasValue)
            {
                var roleRepo = _unitOfWork.GetRepository<Role>();
                var role = await roleRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.RoleId == request.RoleId.Value,
                    asNoTracking: true
                );
                if (role == null)
                    throw new BadHttpRequestException("RoleNotFound");
            }

            // Hash password
            var hashedPassword = PasswordUtil.HashPassword(request.Password);

            // Tạo user mới
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                RoleId = request.RoleId ?? 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await userRepo.InsertAsync(newUser);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("RegisterFailed");

            // Load user với Role để tạo token
            var createdUser = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == newUser.UserId,
                include: q => q.Include(u => u.Role),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("UserNotFoundAfterCreation");

            return await GenerateAuthResponseAsync(createdUser);
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            // Kiểm tra admin từ appsettings TRƯỚC
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (!string.IsNullOrEmpty(adminEmail) &&
                !string.IsNullOrEmpty(adminPassword) &&
                string.Equals(request.Email, adminEmail, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(request.Password, adminPassword, StringComparison.Ordinal))
            {
                // Admin login - không tạo token, trả về AuthResponse với token = null
                return new AuthResponse
                {
                    AccessToken = null,
                    AccessTokenExpiresAt = null,
                    RefreshToken = null,
                    RefreshTokenExpiresAt = null,
                    User = new AuthUserInfo
                    {
                        UserId = 0,
                        Username = "Admin",
                        Email = adminEmail,
                        RoleId = null
                    }
                };
            }

            // Nếu không phải admin, kiểm tra database như bình thường
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Email == request.Email,
                include: q => q.Include(u => u.Role),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("InvalidEmailOrPassword");

            var hashed = PasswordUtil.HashPassword(request.Password);
            if (!string.Equals(user.Password, hashed, StringComparison.Ordinal))
                throw new BadHttpRequestException("InvalidEmailOrPassword");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginWithGoogle(string email, string username)
        {
            if (string.IsNullOrEmpty(email))
                throw new BadHttpRequestException("EmailRequired");

            var userRepo = _unitOfWork.GetRepository<User>();
            
            // Dùng ToLower() để so sánh email giống như Register method
            var normalizedEmail = email.ToLower();
            User? user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Email.ToLower() == normalizedEmail,
                include: q => q.Include(u => u.Role),
                asNoTracking: false
            );

            if (user == null)
            {
                // Tạo user mới
                user = new User
                {
                    Username = string.IsNullOrEmpty(username) ? email : username,
                    Email = email, // Lưu email gốc từ Google
                    Password = PasswordUtil.HashPassword(Guid.NewGuid().ToString()),
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await userRepo.InsertAsync(user);
                try
                {
                    if (await _unitOfWork.CommitAsync() <= 0)
                        throw new BadHttpRequestException("RegisterFailed");
                    
                    // Nếu commit thành công, reload user với Role
                    user = await userRepo.GetFirstOrDefaultAsync(
                        predicate: x => x.UserId == user.UserId,
                        include: q => q.Include(u => u.Role),
                        asNoTracking: true
                    ) ?? throw new BadHttpRequestException("UserNotFoundAfterGoogle");
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    // Duplicate key - có thể do race condition hoặc duplicate email
                    // Thử load lại user bằng email
                    _logger.LogWarning(ex, "Duplicate key error when creating user with email {Email}, attempting to reload", email);
                    
                    user = await userRepo.GetFirstOrDefaultAsync(
                        predicate: x => x.Email.ToLower() == normalizedEmail,
                        include: q => q.Include(u => u.Role),
                        asNoTracking: true
                    );
                    
                    if (user == null)
                    {
                        // Nếu vẫn không tìm thấy, có thể là duplicate key trên primary key (sequence issue)
                        // Hoặc có vấn đề khác
                        _logger.LogError("User not found after duplicate key exception for email {Email}", email);
                        throw new BadHttpRequestException("RegisterFailed");
                    }
                    // Nếu tìm thấy user, tiếp tục với user này
                }
            }
            else if (user.Role == null)
            {
                user.RoleId ??= 1;
                await _unitOfWork.CommitAsync();
                user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == user.UserId,
                    include: q => q.Include(u => u.Role),
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException("UserNotFoundAfterGoogle");
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> Refresh(RefreshTokenRequest request)
        {
            var refreshRepo = _unitOfWork.GetRepository<RefreshToken>();
            var token = await refreshRepo.GetFirstOrDefaultAsync(
                predicate: x => x.RefreshToken1 == request.RefreshToken && x.IsActive == true,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("RefreshTokenNotFound");

            if (token.ExpiresAt <= DateTime.UtcNow)
                throw new BadHttpRequestException("RefreshTokenExpired");

            // Lấy user
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == token.UserId,
                include: q => q.Include(u => u.Role),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("UserNotFound");

            // Rotate refresh token: revoke cũ, cấp mới
            token.IsActive = false;
            token.RevokedAt = DateTime.UtcNow;

            var (newRefresh, newRefreshExp) = GenerateRefreshToken();
            await refreshRepo.InsertAsync(new RefreshToken
            {
                UserId = user.UserId,
                RefreshToken1 = newRefresh,
                ExpiresAt = newRefreshExp,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            var (access, accessExp) = GenerateAccessToken(user);

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok) throw new BadHttpRequestException("RefreshPersistFailed");

            return BuildAuthResponse(user, access, accessExp, newRefresh, newRefreshExp);
        }

        public async Task<bool> Logout(string refreshToken)
        {
            var refreshRepo = _unitOfWork.GetRepository<RefreshToken>();
            var token = await refreshRepo.GetFirstOrDefaultAsync(
                predicate: x => x.RefreshToken1 == refreshToken && x.IsActive == true,
                asNoTracking: false
            );
            if (token == null) return true; // idempotent

            token.IsActive = false;
            token.RevokedAt = DateTime.UtcNow;
            return await _unitOfWork.CommitAsync() > 0;
        }

        private (string token, DateTime expires) GenerateAccessToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"];
            var minutes = int.Parse(jwtSection["AccessTokenMinutes"] ?? "60");

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
        new Claim(ClaimTypes.Role, user.RoleId?.ToString() ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private (string refresh, DateTime expires) GenerateRefreshToken()
        {
            var days = int.Parse(_configuration["Jwt:RefreshTokenDays"] ?? "7");
            var bytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = Convert.ToBase64String(bytes)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty)
                .Replace("=", string.Empty);
            return (token, DateTime.UtcNow.AddDays(days));
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var (accessToken, accessExp) = GenerateAccessToken(user);
            var (refreshToken, refreshExp) = GenerateRefreshToken();

            var refreshRepo = _unitOfWork.GetRepository<RefreshToken>();
            await refreshRepo.InsertAsync(new RefreshToken
            {
                UserId = user.UserId,
                RefreshToken1 = refreshToken,
                ExpiresAt = refreshExp,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok)
                throw new BadHttpRequestException("TokenPersistFailed");

            return BuildAuthResponse(user, accessToken, accessExp, refreshToken, refreshExp);
        }

        private static AuthResponse BuildAuthResponse(User user, string access, DateTime accessExp, string refresh, DateTime refreshExp)
        {
            return new AuthResponse
            {
                AccessToken = access,
                AccessTokenExpiresAt = accessExp,
                RefreshToken = refresh,
                RefreshTokenExpiresAt = refreshExp,
                User = new AuthUserInfo
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    RoleId = user.RoleId
                }
            };
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("UserNotFound");

            // Kiểm tra user có phải Google login không (không có password)
            if (string.IsNullOrEmpty(user.Password))
                throw new BadHttpRequestException("CannotChangePasswordForGoogleAccount");

            // Verify current password
            var currentPasswordHash = PasswordUtil.HashPassword(request.CurrentPassword);
            if (!string.Equals(user.Password, currentPasswordHash, StringComparison.Ordinal))
                throw new BadHttpRequestException("InvalidCurrentPassword");

            // Hash new password
            var newPasswordHash = PasswordUtil.HashPassword(request.NewPassword);
            user.Password = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok)
                throw new BadHttpRequestException("ChangePasswordFailed");

            return true;
        }

        public async Task<bool> ChangeEmail(ChangeEmailRequest request)
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("UserNotFound");

            // Kiểm tra user có phải Google login không (không có password)
            if (string.IsNullOrEmpty(user.Password))
                throw new BadHttpRequestException("CannotChangeEmailForGoogleAccount");

            // Verify password
            var passwordHash = PasswordUtil.HashPassword(request.Password);
            if (!string.Equals(user.Password, passwordHash, StringComparison.Ordinal))
                throw new BadHttpRequestException("InvalidPassword");

            // Kiểm tra email mới đã tồn tại chưa
            var existingUser = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Email.ToLower() == request.NewEmail.ToLower() && x.UserId != userId,
                asNoTracking: true
            );

            if (existingUser != null)
                throw new BadHttpRequestException("EmailAlreadyExists");

            // Update email
            user.Email = request.NewEmail.ToLower();
            user.UpdatedAt = DateTime.UtcNow;

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok)
                throw new BadHttpRequestException("ChangeEmailFailed");

            return true;
        }
    }
}