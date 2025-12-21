using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class TargetLevel
{
    public int LevelId { get; set; }

    public string Name { get; set; } = null!;

    // --- Navigation Property ---
    public virtual ICollection<CourseTargetLevel> CourseTargetLevels { get; set; } = new List<CourseTargetLevel>();
}
