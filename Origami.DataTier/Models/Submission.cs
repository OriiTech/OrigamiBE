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

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual ICollection<SubmissionComment> SubmissionComments { get; set; } = new List<SubmissionComment>();

    public virtual SubmissionFoldingDetail? SubmissionFoldingDetail { get; set; }

    public virtual ICollection<SubmissionImage> SubmissionImages { get; set; } = new List<SubmissionImage>();

    public virtual ICollection<SubmissionLike> SubmissionLikes { get; set; } = new List<SubmissionLike>();

    public virtual ICollection<SubmissionView> SubmissionViews { get; set; } = new List<SubmissionView>();

    public virtual User SubmittedByNavigation { get; set; } = null!;

    public virtual Team? Team { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
