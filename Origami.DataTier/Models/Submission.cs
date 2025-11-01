using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Submission
{
    public int SubmissionId { get; set; }

    public int ChallengeId { get; set; }

    public int? TeamId { get; set; }

    public int SubmittedBy { get; set; }

    public string? FileUrl { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual User SubmittedByNavigation { get; set; } = null!;

    public virtual Team? Team { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
