using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideAccess
{
    public int AccessId { get; set; }

    public int UserId { get; set; }

    public int GuideId { get; set; }

    public DateTime? GrantedAt { get; set; }

    public virtual Guide Guide { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
