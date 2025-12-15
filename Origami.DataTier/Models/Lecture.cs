using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Lecture
{
    public int LectureId { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Type { get; set; } = null!;

    public int? DurationMinutes { get; set; }

    public bool PreviewAvailable { get; set; }

    public string? ContentUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<LectureProgress> LectureProgresses { get; set; } = new List<LectureProgress>();

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
