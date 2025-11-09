using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class GetChallengeResponse
    {
        public int ChallengeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ChallengeType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxTeamSize { get; set; }
        public bool IsTeamBased { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
