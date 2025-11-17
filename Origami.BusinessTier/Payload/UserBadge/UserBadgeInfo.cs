using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.UserBadge
{
    public class UserBadgeInfo
    {
        public int UserId { get; set; }
        public int BadgeId { get; set; }
        public DateTime? EarnedAt { get; set; }
    }
}
