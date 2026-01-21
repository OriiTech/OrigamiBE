using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.UserProfile;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement
{
    public class UserProfileService : BaseService<UserProfileService>, IUserProfileService
    {
        public UserProfileService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<UserProfileService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
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

            return new UserProfileResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.UserProfile?.DisplayName ?? user.Username,
                AvatarUrl = user.UserProfile?.AvatarUrl,
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

            if (request.AvatarUrl != null)
            {
                profile.AvatarUrl = request.AvatarUrl.Trim();
            }

            if (request.Bio != null)
            {
                profile.Bio = request.Bio.Trim();
            }

            user.UpdatedAt = DateTime.UtcNow;

            var ok = await _unitOfWork.CommitAsync() > 0;
            if (!ok)
                throw new BadHttpRequestException("UpdateProfileFailed");

            return new UserProfileResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DisplayName = profile.DisplayName ?? user.Username,
                AvatarUrl = profile.AvatarUrl,
                Bio = profile.Bio
            };
        }
    }
}

