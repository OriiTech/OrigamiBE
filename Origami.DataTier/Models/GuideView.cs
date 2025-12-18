using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideView
{
    public int ViewId { get; set; }

    public int GuideId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Guide Guide { get; set; } = null!;

    public virtual User? User { get; set; }
}
