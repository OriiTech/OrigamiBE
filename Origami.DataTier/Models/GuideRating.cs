using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideRating
{
    public int GuideId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Guide Guide { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
