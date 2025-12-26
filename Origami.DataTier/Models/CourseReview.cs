using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class CourseReview
{
    public int ReviewId { get; set; }

    public int? CourseId { get; set; }

    public int? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? LikeCount { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<ReviewResponse> ReviewResponses { get; set; } = new List<ReviewResponse>();

    public virtual User? User { get; set; }
}
