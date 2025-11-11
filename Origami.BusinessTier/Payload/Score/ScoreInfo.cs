using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Score
{
    public class ScoreInfo
    {
        public int SubmissionId { get; set; }
        public decimal Score { get; set; }
        public int ScoreBy { get; set; }
    }
}
