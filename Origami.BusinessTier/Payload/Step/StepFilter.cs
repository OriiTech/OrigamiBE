using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Step
{
    public class StepFilter
    {
        public int? GuideId { get; set; }
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
