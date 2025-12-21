using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuideCreator
{
    public int CreatorId { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
}
