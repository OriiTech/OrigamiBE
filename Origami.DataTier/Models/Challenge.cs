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

    public string? Status { get; set; }

    public string? Phase { get; set; }

    public string? Level { get; set; }

    public string? Theme { get; set; }

    public string? PromoPhoto { get; set; }

    public bool? IsFree { get; set; }

    public decimal? EntryFee { get; set; }

    public decimal? PrizePool { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsFeatured { get; set; }

    public virtual ICollection<ChallengeOtherRequirement> ChallengeOtherRequirements { get; set; } = new List<ChallengeOtherRequirement>();

    public virtual ICollection<ChallengePrize> ChallengePrizes { get; set; } = new List<ChallengePrize>();

    public virtual ChallengeRequirement? ChallengeRequirement { get; set; }

    public virtual ICollection<ChallengeRule> ChallengeRules { get; set; } = new List<ChallengeRule>();

    public virtual ChallengeSchedule? ChallengeSchedule { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
