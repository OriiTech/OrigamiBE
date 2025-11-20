using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.TeamMember
{
    public class TeamMemberIdentity
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
    }

    public class BulkAddTeamMemberRequest
    {
        public int TeamId { get; set; }
        public List<TeamMemberIdentity> Members { get; set; } = new();
    }
}
