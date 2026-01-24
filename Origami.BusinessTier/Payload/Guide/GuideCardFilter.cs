using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GuideCardFilter
    {
        public string? Title { get; set; }
        public string? CreatorName { get; set; }

        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public bool? PaidOnly { get; set; }

        public double? MinRating { get; set; }
        public int? MinViews { get; set; }

        public bool? Bestseller { get; set; }
        public bool? Trending { get; set; }
        public bool? IsNew { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? SortBy { get; set; } // "price", "rating"
        public string? SortOrder { get; set; } // "asc", "desc"
    }

}
