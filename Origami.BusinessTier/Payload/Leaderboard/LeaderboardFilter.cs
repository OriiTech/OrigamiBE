using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Leaderboard
{
    public class LeaderboardFilter
    {
        public int? ChallengeId { get; set; }
        public int? TeamId { get; set; }
        public int? UserId { get; set; }
        public decimal? MinTotalScore { get; set; }
        public decimal? MaxTotalScore { get; set; }
    }
}
