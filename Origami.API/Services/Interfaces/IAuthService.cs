using Origami.BusinessTier.Payload.Auth;

namespace Origami.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> Refresh(RefreshTokenRequest request);
        Task<bool> Logout(string refreshToken);
    }
}