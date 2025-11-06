using Origami.BusinessTier.Payload.Auth;

namespace Origami.API.Services.Interfaces
{
    public interface IPasswordHashService
    {
        Task<HashPasswordResponse> HashAsync(HashPasswordRequest request);
    }
}