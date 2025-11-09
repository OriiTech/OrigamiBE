using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GuideInfo
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int AuthorId { get; set; }
        public int? OrigamiId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int>? CategoryIds { get; set; } = new();
    }
}
