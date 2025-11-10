using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.TeamMember
{
    public class GetTeamMemberResponse
    {
        public int TeamMemberId { get; set; }
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}