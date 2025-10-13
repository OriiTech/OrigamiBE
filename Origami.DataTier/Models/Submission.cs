using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Submission
{
    public int SubmissionId { get; set; }

    public int? ChallengeId { get; set; }

    public int? ParticipantId { get; set; }

    public string? SubmissionLink { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Challenge? Challenge { get; set; }

    public virtual EventParticipant? Participant { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
