using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengeOtherRequirement
{
    public int Id { get; set; }

    public int? ChallengeId { get; set; }

    public string? Content { get; set; }

    public virtual Challenge? Challenge { get; set; }
}
