using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Badge
{
    public int BadgeId { get; set; }

    public string BadgeName { get; set; } = null!;

    public string? BadgeDescription { get; set; }

    public string? ConditionType { get; set; }

    public string? ConditionValue { get; set; }

    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

    public virtual ICollection<ChallengePrize> Prizes { get; set; } = new List<ChallengePrize>();
}
