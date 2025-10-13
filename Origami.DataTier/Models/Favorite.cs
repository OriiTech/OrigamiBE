using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int GuideId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Guide Guide { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
