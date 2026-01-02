using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengeRequirement
{
    public int ChallengeId { get; set; }

    public string? PaperRequirements { get; set; }

    public string? FoldingConstraints { get; set; }

    public string? PhotographyRequirements { get; set; }

    public string? ModelRequirements { get; set; }

    public int? MaximumSubmissions { get; set; }

    public int? TeamSize { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
