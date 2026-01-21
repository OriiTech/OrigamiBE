using System;

namespace Origami.BusinessTier.Payload.UserProfile
{
    public class UpdateUserProfileRequest
    {
        /// <summary>
        /// Tên hiển thị trên app. Nếu null thì giữ nguyên.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Link ảnh avatar (đã upload lên Firebase). Nếu null thì giữ nguyên.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Giới thiệu / bio. Nếu null thì giữ nguyên.
        /// </summary>
        public string? Bio { get; set; }
    }
}

