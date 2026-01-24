using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class PaymentGuideResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public int TransactionId { get; set; }
    }
}
