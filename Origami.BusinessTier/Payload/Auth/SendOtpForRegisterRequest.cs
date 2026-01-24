using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Auth
{
    public class SendOtpForRegisterRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
    }
}
