using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Email
{
    public class FeedbackEmailRequest
    {
        // Email không còn cần thiết vì sẽ lấy từ JWT token của user đang đăng nhập
        // Giữ lại để backward compatibility, nhưng không required
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject must not exceed 200 characters")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content must not exceed 5000 characters")]
        public string Content { get; set; } = null!;
    }
}
