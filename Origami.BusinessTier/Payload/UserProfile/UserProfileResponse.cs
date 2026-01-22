using System;

namespace Origami.BusinessTier.Payload.UserProfile
{
    public class UserProfileResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? RoleId { get; set; }

        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}

