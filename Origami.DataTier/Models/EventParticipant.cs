using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class EventParticipant
{
    public int ParticipantId { get; set; }

    public int? ChallengeId { get; set; }

    public int? UserId { get; set; }

    public int? TeamId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Challenge? Challenge { get; set; }

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual Team? Team { get; set; }

    public virtual User? User { get; set; }
}
