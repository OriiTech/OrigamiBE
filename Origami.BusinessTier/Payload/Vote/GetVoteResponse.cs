using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Vote
{
    public class GetVoteResponse
    {
        public int VoteId { get; set; }
        public int SubmissionId { get; set; }
        public int UserId { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
