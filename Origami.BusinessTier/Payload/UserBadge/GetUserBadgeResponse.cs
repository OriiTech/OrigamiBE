using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.UserBadge
{
    public class GetUserBadgeResponse
    {
        public int UserId { get; set; }
        public int BadgeId { get; set; }
        public DateTime? EarnedAt { get; set; }

        public string? BadgeName { get; set; }
        public string? BadgeDescription { get; set; }
    }
}
