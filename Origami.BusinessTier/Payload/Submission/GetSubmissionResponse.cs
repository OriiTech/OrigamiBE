using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Submission
{
    public class GetSubmissionResponse
    {
        public int SubmissionId { get; set; }
        public int ChallengeId { get; set; }
        public int? TeamId { get; set; }
        public int SubmittedBy { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}

