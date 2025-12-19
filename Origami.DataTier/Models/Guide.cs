using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Guide
{
    public int GuideId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int AuthorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? OrigamiId { get; set; }

    public string? Subtitle { get; set; }

    public bool PaidOnly { get; set; }

    public bool Bestseller { get; set; }

    public bool Trending { get; set; }

    public bool IsNew { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<GuideAccess> GuideAccesses { get; set; } = new List<GuideAccess>();

    public virtual GuidePreview? GuidePreview { get; set; }

    public virtual ICollection<GuidePromoPhoto> GuidePromoPhotos { get; set; } = new List<GuidePromoPhoto>();

    public virtual ICollection<GuideRating> GuideRatings { get; set; } = new List<GuideRating>();

    public virtual GuideRequirement? GuideRequirement { get; set; }

    public virtual ICollection<GuideView> GuideViews { get; set; } = new List<GuideView>();

    public virtual Origami? Origami { get; set; }

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
