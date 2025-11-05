using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GuideFilter
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? AuthorId { get; set; }
        public int? OrigamiId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
