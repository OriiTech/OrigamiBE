using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuidePreview
{
    public int GuideId { get; set; }

    public bool VideoAvailable { get; set; }

    public string? VideoUrl { get; set; }

    public virtual Guide Guide { get; set; } = null!;
}
