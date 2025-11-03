using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ChallengeType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxTeamSize { get; set; }

    public bool IsTeamBased { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
