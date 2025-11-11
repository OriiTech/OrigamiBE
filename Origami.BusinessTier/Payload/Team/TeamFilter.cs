using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Team
{
    public class TeamFilter
    {
        public string? TeamName { get; set; }
        public int? ChallengeId { get; set; }
    }
}
