using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Auth
{
    public class ChangeEmailRequest
    {
        [Required(ErrorMessage = "New email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
