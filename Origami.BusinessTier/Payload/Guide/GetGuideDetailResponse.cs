using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GetGuideDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }

        public CreatorDto Creator { get; set; }

        public string? Level { get; set; }
        public List<string> Category { get; set; }

        public int TotalViews { get; set; }
        public int TotalReviews { get; set; }
        public int TotalBookmarks { get; set; }

        public RatingDto Rating { get; set; }

        public PriceDto Price { get; set; }
        public ProductPreviewDto ProductPreview { get; set; }
        public ContentDto Content { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool Bestseller { get; set; }
        public bool Trending { get; set; }
        public bool New { get; set; }
        
        public OrigamiDto? Origami { get; set; }
        public bool HasAccess { get; set; }
    }
    
    public class OrigamiDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreatorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public string? Bio { get; set; }
    }
    public class RatingDto
    {
        public double Average { get; set; }
        public int Count { get; set; }
        public RatingDistributionDto Distribution { get; set; }
    }

    public class RatingDistributionDto
    {
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }

    public class PriceDto
    {
        public decimal Amount { get; set; }
        public bool PaidOnly { get; set; }
    }

    public class ProductPreviewDto
    {
        public bool VideoAvailable { get; set; }
        public string? VideoUrl { get; set; }
        public List<PromoPhotoDto> PromoPhotos { get; set; }
    }

    public class PromoPhotoDto
    {
        public int Order { get; set; }
        public string Url { get; set; }
    }
    public class ContentDto
    {
        public List<StepDto> Steps { get; set; }
        public int TotalSteps { get; set; }
        public RequirementDto Requirements { get; set; }
    }
    public class StepDto
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        /// <summary>Nội dung mẹo từ StepTip, theo DisplayOrder.</summary>
        public List<string> Tips { get; set; } = new List<string>();

        public List<MediaDto> MediaUrl { get; set; }
    }

    public class MediaDto
    {
        public int Order { get; set; }
        public string Url { get; set; }
        public string? Note { get; set; }
    }
    public class RequirementDto
    {
        public string? PaperType { get; set; }
        public string? PaperSize { get; set; }
        public List<string> Color { get; set; }
        public List<string> Tools { get; set; }
    }

}
