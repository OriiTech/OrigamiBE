using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Step
{
    public int StepId { get; set; }

    public int GuideId { get; set; }

    public int StepNumber { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Guide Guide { get; set; } = null!;
}
