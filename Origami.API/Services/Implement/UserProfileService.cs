using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.UserProfile;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Origami.API.Services.Implement
{
    public class UserProfileService : BaseService<UserProfileService>, IUserProfileService
    {
        private readonly IUploadService _uploadService;

        public UserProfileService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<UserProfileService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUploadService uploadService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
        }

        private string? ExtractObjectNameFromUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Pattern: https://firebasestorage.googleapis.com/v0/b/{bucket}/o/{objectName}?alt=media
            var match = Regex.Match(url, @"/o/([^?]+)");
            if (match.Success)
            {
                return Uri.UnescapeDataString(match.Groups[1].Value);
            }
            return null;
        }

        private async Task<string?> GetSignedAvatarUrlAsync(string? avatarUrl)
        {
            if (string.IsNullOrEmpty(avatarUrl)) return null;

            try
            {
                var objectName = ExtractObjectNameFromUrl(avatarUrl);
                if (objectName != null)
                {
                    // Tạo signed URL với thời hạn 7 ngày
                    return await _uploadService.GetSignedUrlAsync(objectName, TimeSpan.FromDays(7));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create signed URL for avatar: {AvatarUrl}", avatarUrl);
            }

            // Fallback: trả về URL gốc nếu không tạo được signed URL
            return avatarUrl;
        }

        public async Task<UserProfileResponse> GetMyProfileAsync()
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId,
                include: q => q.Include(u => u.UserProfile),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("UserNotFound");

            var avatarUrl = await GetSignedAvatarUrlAsync(user.UserProfile?.AvatarUrl);

            return new UserProfileResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.UserProfile?.DisplayName ?? user.Username,
                AvatarUrl = avatarUrl,
                Bio = user.UserProfile?.Bio
            };
        }

        public async Task<UserProfileResponse> UpdateMyProfileAsync(UpdateUserProfileRequest request)
        {
            int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

            var userRepo = _unitOfWork.GetRepository<User>();
            var profileRepo = _unitOfWork.GetRepository<UserProfile>();

            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId,
                include: q => q.Include(u => u.UserProfile),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("UserNotFound");

            var profile = user.UserProfile;
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    DisplayName = user.Username
                };
                await profileRepo.InsertAsync(profile);
                user.UserProfile = profile;
            }

            if (request.DisplayName != null)
            {
                profile.DisplayName = request.DisplayName.Trim();
            }

            // Upload avatar file lên Firebase nếu có
            if (request.AvatarFile != null && request.AvatarFile.Length > 0)
            {
                // Validate file type (chỉ cho phép image)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(request.AvatarFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new BadHttpRequestException("Only image files are allowed (jpg, jpeg, png, gif, webp)");
                }

                // Validate file size (max 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (request.AvatarFile.Length > maxFileSize)
                {
                    throw new BadHttpRequestException("File size must be less than 5MB");
                }

                // Upload lên Firebase Storage
                var uploadedAvatarUrl = await _uploadService.UploadAsync(request.AvatarFile, "avatars");
                profile.AvatarUrl = uploadedAvatarUrl;
                _logger.LogInformation($"Avatar uploaded for user {userId}: {uploadedAvatarUrl}");
            }

            if (request.Bio != null)
            {
                profile.Bio = request.Bio.Trim();
            }

            user.UpdatedAt = DateTime.UtcNow;

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok)
                throw new BadHttpRequestException("UpdateProfileFailed");

            var signedAvatarUrl = await GetSignedAvatarUrlAsync(profile.AvatarUrl);

            return new UserProfileResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DisplayName = profile.DisplayName ?? user.Username,
                AvatarUrl = signedAvatarUrl,
                Bio = profile.Bio
            };
        }
    }
}

