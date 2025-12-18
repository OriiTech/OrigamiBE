using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class StepTip
{
    public int TipId { get; set; }

    public int StepId { get; set; }

    public int DisplayOrder { get; set; }

    public string Content { get; set; } = null!;

    public virtual Step Step { get; set; } = null!;
}
