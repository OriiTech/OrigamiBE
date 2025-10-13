using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Leaderboard
{
    public int LeaderboardId { get; set; }

    public int? ChallengeId { get; set; }

    public int? ParticipantId { get; set; }

    public decimal? Score { get; set; }

    public virtual Challenge? Challenge { get; set; }

    public virtual EventParticipant? Participant { get; set; }
}
