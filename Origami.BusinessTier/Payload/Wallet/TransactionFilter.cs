namespace Origami.BusinessTier.Payload.Wallet;

public class TransactionFilter
{
    public int? WalletId { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

