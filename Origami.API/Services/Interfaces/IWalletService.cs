using Origami.BusinessTier.Payload.Wallet;

namespace Origami.API.Services.Interfaces;

public interface IWalletService
{
    Task<WalletResponse> GetMyWalletAsync();
    Task<List<TransactionResponse>> GetMyTransactionsAsync();
    Task<TopUpResponse> CreateTopUpTransactionAsync(TopUpRequest request);
    Task<bool> ProcessVnPayCallbackAsync(Dictionary<string, string> vnpayData);
}
