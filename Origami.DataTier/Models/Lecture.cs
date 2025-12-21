using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Lecture
{
    public int LectureId { get; set; }
    public int? LessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public int? DurationMinutes { get; set; }
    public bool? PreviewAvailable { get; set; }
    public DateTime? CreatedAt { get; set; }

    // --- Navigation Properties ---
    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
    public virtual ICollection<LectureProgress> LectureProgresses { get; set; } = new List<LectureProgress>();
}
