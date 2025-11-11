using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Leaderboard
{
    public class GetLeaderboardResponse
    {
        public int LeaderboardId { get; set; }
        public int ChallengeId { get; set; }
        public int? TeamId { get; set; }
        public int? UserId { get; set; }
        public decimal TotalScore { get; set; }
        public int? Rank { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
