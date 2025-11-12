using AutoMapper;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class WalletService : BaseService<WalletService>, IWalletService
    {
        public WalletService(IUnitOfWork<OrigamiDbContext> uow, ILogger<WalletService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }
        public async Task<GetWalletResponse> GetWalletByUserId(int userId)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("WalletNotFound");
            return _mapper.Map<GetWalletResponse>(wallet);
        }
    }
}
