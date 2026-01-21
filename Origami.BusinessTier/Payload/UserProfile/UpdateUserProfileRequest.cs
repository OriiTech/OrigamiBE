using Microsoft.AspNetCore.Http;

namespace Origami.BusinessTier.Payload.UserProfile
{
    public class UpdateUserProfileRequest
    {
        public string? DisplayName { get; set; }
        public IFormFile? AvatarFile { get; set; }
        public string? Bio { get; set; }
    }
}

