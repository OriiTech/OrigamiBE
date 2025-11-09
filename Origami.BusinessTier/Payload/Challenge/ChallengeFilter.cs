using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeFilter
    {
        public string? Title { get; set; }
        public string? ChallengeType { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsTeamBased { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
    }
}
