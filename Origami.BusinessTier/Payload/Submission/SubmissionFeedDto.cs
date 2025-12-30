using Origami.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Submission
{
    public class SubmissionFeedDto
    {
        public int ChallengeId { get; set; }
        public string ChallengeTitle { get; set; } = null!;
        public string CurrentPhase { get; set; } = null!;
        public string? VotingEndsIn { get; set; }

        public IPaginate<SubmissionFeedItemDto> Submissions { get; set; }


        public SubmissionFeedUserContextDto UserContext { get; set; } = null!;
    }
    public class SubmissionFeedUserContextDto
    {
        public bool CanSubmit { get; set; }
        public bool HasSubmissions { get; set; }
        public bool CanVote { get; set; }
        public bool IsJudge { get; set; }
        public bool IsOrganizer { get; set; }
    }

}
