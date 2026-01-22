namespace Origami.BusinessTier.Payload.Wallet;

public class TopUpResponse
{
    public string PaymentUrl { get; set; } = null!;
    public int TransactionId { get; set; }
}
