using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string? Bio { get; set; }

    public decimal? RatingAvg { get; set; }

    public int TotalReviews { get; set; }

    public int TotalCourses { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User InstructorNavigation { get; set; } = null!;
}
