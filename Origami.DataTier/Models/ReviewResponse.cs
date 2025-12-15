using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ReviewResponse
{
    public int ResponseId { get; set; }

    public int ReviewId { get; set; }

    public int InstructorId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual CourseReview Review { get; set; } = null!;
}
