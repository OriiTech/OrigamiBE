using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IWalletService
    {
        Task<GetWalletResponse> GetWalletByUserId(int userId);
    }
}
