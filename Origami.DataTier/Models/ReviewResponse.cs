using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ReviewResponse
{
    public int ResponseId { get; set; }

    public int ReviewId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual CourseReview Review { get; set; } = null!;

    public virtual User? User { get; set; }
}
