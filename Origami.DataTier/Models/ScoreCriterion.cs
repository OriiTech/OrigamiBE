using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ScoreCriterion
{
    public int ScoreId { get; set; }

    public decimal? Creativity { get; set; }

    public decimal? Execution { get; set; }

    public decimal? Theme { get; set; }

    public decimal? Difficulty { get; set; }

    public decimal? Presentation { get; set; }

    public virtual Score Score { get; set; } = null!;
}
