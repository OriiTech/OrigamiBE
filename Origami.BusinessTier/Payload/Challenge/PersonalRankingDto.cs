using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class PersonalRankingDto
    {
        public int ChallengeId { get; set; }
        public bool HasSubmission { get; set; }

        public int? Rank { get; set; }
        public double? Score { get; set; }

        public int VotesReceived { get; set; }
        public int CommentsReceived { get; set; }
        public int SubmissionViews { get; set; }
    }

}
