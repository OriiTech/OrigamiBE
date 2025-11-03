using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Origami
{
    public int OrigamiId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Guide> Guides { get; set; } = new List<Guide>();
}
