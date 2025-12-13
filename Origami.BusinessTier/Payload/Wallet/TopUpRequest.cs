namespace Origami.BusinessTier.Payload.Wallet;

public class TopUpRequest
{
    public decimal Amount { get; set; } // Số tiền VND muốn nạp
    public string? ReturnUrl { get; set; } // URL để VNPAY redirect về sau khi thanh toán
}

