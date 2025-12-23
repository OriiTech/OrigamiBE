using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeListFilter
    {
        public string? Keyword { get; set; }          
        public string? Status { get; set; }
        public string? Phase { get; set; }
        public string? Level { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsFree { get; set; }
    }


}
