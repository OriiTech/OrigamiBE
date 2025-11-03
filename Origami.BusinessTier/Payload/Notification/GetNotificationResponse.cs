using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Notification
{
    public class GetNotificationResponse
    {
        public int NotificationId { get; set; }

        public int? UserId { get; set; }

        public string? Content { get; set; }

        public bool? IsRead { get; set; }

        public DateTime? CreatedAt { get; set; } 
    }
}
