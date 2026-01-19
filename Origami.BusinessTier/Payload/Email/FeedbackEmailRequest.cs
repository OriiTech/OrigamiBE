using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Email
{
    public class FeedbackEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject must not exceed 200 characters")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content must not exceed 5000 characters")]
        public string Content { get; set; } = null!;
    }
}
