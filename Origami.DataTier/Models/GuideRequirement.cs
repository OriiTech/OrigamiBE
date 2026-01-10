using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideRequirement
{
    public int GuideId { get; set; }

    public string? PaperType { get; set; }

    public string? PaperSize { get; set; }

    public string? Colors { get; set; }

    public string? Tools { get; set; }

    public virtual Guide Guide { get; set; } = null!;
}
