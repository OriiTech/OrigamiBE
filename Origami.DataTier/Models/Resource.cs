using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Resource
{
    public int ResourceId { get; set; }

    public int LectureId { get; set; }

    public string Title { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Lecture Lecture { get; set; } = null!;
}
