using AutoMapper;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Auth;
using Origami.BusinessTier.Utils;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement
{
    public class PasswordHashService : BaseService<PasswordHashService>, IPasswordHashService
    {
        public PasswordHashService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<PasswordHashService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        ) : base(unitOfWork, logger, mapper, httpContextAccessor) { }

        public Task<HashPasswordResponse> HashAsync(HashPasswordRequest request)
        {
            var hash = PasswordUtil.HashPassword(request.Password);
            return Task.FromResult(new HashPasswordResponse
            {
                Hash = hash,
                Algorithm = "SHA256"
            });
        }
    }
}