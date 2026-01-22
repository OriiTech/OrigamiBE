namespace Origami.BusinessTier.Payload.Wallet;

public class TransactionResponse
{
    public int TransactionId { get; set; }
    public int? SenderWalletId { get; set; }
    public int? ReceiverWalletId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public int? OrderId { get; set; }
}
