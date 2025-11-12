using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Wallet
{
    public class GetWalletResponse
    {
        public int WalletId { get; set; }

        public int UserId { get; set; }

        public decimal? Balance { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
