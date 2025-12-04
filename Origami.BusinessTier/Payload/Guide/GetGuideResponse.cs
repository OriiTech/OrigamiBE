using Origami.BusinessTier.Payload.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GetGuideResponse
    {
        public int GuideId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public int? OrigamiId { get; set; }
        public string? OrigamiName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int>? CategoryIds { get; set; } = new();
        public List<StepInfo> Steps { get; set; } = new();
    }
}
