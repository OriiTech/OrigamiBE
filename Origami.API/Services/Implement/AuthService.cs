using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Auth;
using Origami.BusinessTier.Utils;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.Email == request.Email,
                include: q => q.Include(u => u.Role),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("InvalidEmailOrPassword");

            var hashed = PasswordUtil.HashPassword(request.Password);
            if (!string.Equals(user.Password, hashed, StringComparison.Ordinal))
                throw new BadHttpRequestException("InvalidEmailOrPassword");

            // Tạo tokens
            var (accessToken, accessExp) = GenerateAccessToken(user);
            var (refreshToken, refreshExp) = GenerateRefreshToken();

            // Lưu refresh token
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
            if (!ok) throw new BadHttpRequestException("LoginPersistFailed");

            return BuildAuthResponse(user, accessToken, accessExp, refreshToken, refreshExp);
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
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
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
    }
}