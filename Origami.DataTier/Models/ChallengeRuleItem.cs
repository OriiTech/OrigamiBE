using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengeRuleItem
{
    public int ItemId { get; set; }

    public int? RuleId { get; set; }

    public string? Content { get; set; }

    public virtual ChallengeRule? Rule { get; set; }
}
