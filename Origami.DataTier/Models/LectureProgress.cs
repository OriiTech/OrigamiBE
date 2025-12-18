using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class LectureProgress
{
    public int LectureId { get; set; }

    public int UserId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Lecture Lecture { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
