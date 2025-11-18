using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeRegisterRequest
    {
        public int ChallengeId { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
    }
}
