using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class SubmissionSnapshot
{
    public int SnapshotId { get; set; }

    public int ChallengeId { get; set; }

    public int SubmissionId { get; set; }

    public int UserId { get; set; }

    public int? TeamId { get; set; }

    public int Rank { get; set; }

    public bool IsLocked { get; set; }

    public decimal TotalScore { get; set; }

    public decimal JudgeScore { get; set; }

    public decimal CommunityScore { get; set; }

    public string JudgeCriteriaJson { get; set; } = null!;

    public string JudgeScoresJson { get; set; } = null!;

    public string CommunityStatsJson { get; set; } = null!;

    public DateTime SnapshotAt { get; set; }

    public int CreatedBy { get; set; }
}
