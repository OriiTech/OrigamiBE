using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Notification
{
    public class NotificationFilter
    {
        public bool? IsRead { get; set; } = null!;
        //public DateTime? CreatedAfter { get; set; } = null!;
        //public DateTime? CreatedBefore { get; set; } = null!;
        public DateTime? CreatedAt { get; set; } = null!;
    }
}
