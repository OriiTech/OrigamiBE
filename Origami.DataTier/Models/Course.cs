using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? TeacherId { get; set; }

    public string? Language { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool? Bestseller { get; set; }

    public virtual ICollection<CourseAccess> CourseAccesses { get; set; } = new List<CourseAccess>();

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<CourseTargetLevel> CourseTargetLevels { get; set; } = new List<CourseTargetLevel>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();

    public virtual User? Teacher { get; set; }
}
