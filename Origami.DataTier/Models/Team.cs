using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public int ChallengeId { get; set; }

    public string TeamName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
