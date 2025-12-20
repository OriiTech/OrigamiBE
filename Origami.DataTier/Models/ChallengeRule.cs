using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengeRule
{
    public int RuleId { get; set; }

    public int? ChallengeId { get; set; }

    public string? Section { get; set; }

    public virtual Challenge? Challenge { get; set; }

    public virtual ICollection<ChallengeRuleItem> ChallengeRuleItems { get; set; } = new List<ChallengeRuleItem>();
}
