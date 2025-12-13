namespace Origami.BusinessTier.Payload.Wallet;

public class TopUpResponse
{
    public string PaymentUrl { get; set; } = string.Empty; // URL thanh toán VNPAY
    public string TransactionCode { get; set; } = string.Empty; // Mã giao dịch
}

