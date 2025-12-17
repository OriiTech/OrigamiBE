namespace Origami.BusinessTier.Payload.Wallet;

// Filter dùng cho API "My Transactions" của user hiện tại (không cần WalletId)
public class MyTransactionFilter
{
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}


