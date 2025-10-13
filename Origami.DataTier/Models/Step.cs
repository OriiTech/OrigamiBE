using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Step
{
    public int StepId { get; set; }

    public int GuideId { get; set; }

    public int StepNumber { get; set; }

    public string? Content { get; set; }

    public virtual Guide Guide { get; set; } = null!;
}
