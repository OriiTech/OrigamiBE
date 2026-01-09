using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{

    public class ChallengeJudgeCommandDto
    {
        public int ChallengeId { get; set; }
        public int UserId { get; set; }
    }

}
