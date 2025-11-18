using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Badge
{
    public class BadgeFilter
    {
        public string? BadgeName { get; set; }
        public string? ConditionType { get; set; }
    }
}
