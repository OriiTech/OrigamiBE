using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class ChallengeSchedule
{
    public int ChallengeId { get; set; }

    public DateTime? RegistrationStart { get; set; }

    public DateTime? SubmissionStart { get; set; }

    public DateTime? SubmissionEnd { get; set; }

    public DateTime? VotingStart { get; set; }

    public DateTime? VotingEnd { get; set; }

    public DateTime? ResultsDate { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
