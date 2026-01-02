using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Score
{
    public int ScoreId { get; set; }

    public int SubmissionId { get; set; }

    public decimal Score1 { get; set; }

    public DateTime? ScoreAt { get; set; }

    public int ScoreBy { get; set; }

    public virtual User ScoreByNavigation { get; set; } = null!;

    public virtual ScoreCriterion? ScoreCriterion { get; set; }

    public virtual Submission Submission { get; set; } = null!;
}
