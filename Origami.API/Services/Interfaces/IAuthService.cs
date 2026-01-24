using Origami.BusinessTier.Payload.Auth;
using Origami.BusinessTier.Payload.User;

namespace Origami.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task SendOtpForRegisterAsync(string email);
        Task<AuthResponse> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> LoginWithGoogle(string email, string username);
        Task<AuthResponse> Refresh(RefreshTokenRequest request);
        Task<bool> Logout(string refreshToken);
        Task<bool> ChangePassword(ChangePasswordRequest request);
        Task<bool> ChangeEmail(ChangeEmailRequest request);
    }
}