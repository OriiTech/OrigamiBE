using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideCategory
{
    public int GuideId { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Guide Guide { get; set; } = null!;
}
