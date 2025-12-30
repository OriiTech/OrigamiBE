using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Email
{
    public class TestEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;
    }
}

