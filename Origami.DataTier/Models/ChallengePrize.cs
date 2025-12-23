using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengePrize
{
    public int PrizeId { get; set; }

    public int ChallengeId { get; set; }

    public int Rank { get; set; }

    public decimal? Cash { get; set; }

    public string? Description { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();
}
