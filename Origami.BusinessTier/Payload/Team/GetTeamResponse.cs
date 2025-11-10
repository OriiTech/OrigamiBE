using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Team
{
    public class GetTeamResponse
    {
        public int TeamId { get; set; }
        public int ChallengeId { get; set; }
        public string? ChallengeTitle { get; set; }
        public string TeamName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public int MemberCount { get; set; }
    }
}
