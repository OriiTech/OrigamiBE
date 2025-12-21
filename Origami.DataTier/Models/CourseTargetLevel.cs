using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class CourseTargetLevel
{
    public int CourseId { get; set; }

    public int LevelId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual TargetLevel Level { get; set; } = null!;
}
