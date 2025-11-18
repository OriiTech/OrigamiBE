using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Badge
{
    public class GetBadgeResponse
    {
        public int BadgeId { get; set; }
        public string BadgeName { get; set; } = null!;
        public string? BadgeDescription { get; set; }
        public string? ConditionType { get; set; }
        public string? ConditionValue { get; set; }
    }
}
