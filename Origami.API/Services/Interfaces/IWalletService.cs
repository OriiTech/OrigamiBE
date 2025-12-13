using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces;

public interface IWalletService
{
    Task<GetWalletResponse> GetMyWallet();
    Task<GetWalletResponse> GetWalletById(int id);
    Task<TopUpResponse> TopUpWallet(TopUpRequest request);
    Task<bool> ProcessVnpayCallback(Dictionary<string, string> vnpayData);
    Task<IPaginate<TransactionResponse>> GetMyTransactions(TransactionFilter filter, PagingModel pagingModel);
    Task<IPaginate<TransactionResponse>> GetAllTransactions(TransactionFilter filter, PagingModel pagingModel);
}

