using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GetGuideCardResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public string CreatorName { get; set; } = null!;
        public string? CreatorImage { get; set; }

        public List<CategoryDto> Category { get; set; } = new();

        public int TotalViews { get; set; }
        public double Rating { get; set; }

        public decimal? Price { get; set; }
        public bool PaidOnly { get; set; }

        public List<string> PromoPhotos { get; set; } = new();

        public DateTime? CreatedDate { get; set; }

        public bool Bestseller { get; set; }
        public bool Trending { get; set; }
        public bool New { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

}
