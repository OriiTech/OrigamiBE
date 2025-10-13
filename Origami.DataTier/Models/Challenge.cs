using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ChallengeType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
