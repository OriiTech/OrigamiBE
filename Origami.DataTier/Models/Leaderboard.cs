using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Leaderboard
{
    public int LeaderboardId { get; set; }

    public int ChallengeId { get; set; }

    public int? TeamId { get; set; }

    public int? UserId { get; set; }

    public decimal TotalScore { get; set; }

    public int? Rank { get; set; }

    public DateTime UpdatedAt { get; set; }

    public decimal? JudgeScore { get; set; }

    public decimal? VoteScore { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual Team? Team { get; set; }

    public virtual User? User { get; set; }
}
