using System.ComponentModel.DataAnnotations;

namespace Origami.BusinessTier.Payload.Wallet;

public class TopUpRequest
{
    [Required]
    [Range(10000, 100000000, ErrorMessage = "Số tiền nạp phải từ 10,000 VNĐ đến 100,000,000 VNĐ")]
    public decimal Amount { get; set; }
}
