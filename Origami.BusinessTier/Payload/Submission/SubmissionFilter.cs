using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Submission
{
    public class SubmissionFilter
    {
        public int? ChallengeId { get; set; }
        public int? TeamId { get; set; }
        public int? SubmittedBy { get; set; }
    }
}
