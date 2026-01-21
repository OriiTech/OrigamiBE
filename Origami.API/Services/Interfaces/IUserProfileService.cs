using Origami.BusinessTier.Payload.UserProfile;

namespace Origami.API.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetMyProfileAsync();
        Task<UserProfileResponse> UpdateMyProfileAsync(UpdateUserProfileRequest request);
    }
}

