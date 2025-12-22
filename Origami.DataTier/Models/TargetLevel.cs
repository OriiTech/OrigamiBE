using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class TargetLevel
{
    public int LevelId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
