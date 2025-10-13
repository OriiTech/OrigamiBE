using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Vote
{
    public int VoteId { get; set; }

    public int? SubmissionId { get; set; }

    public int? UserId { get; set; }

    public DateTime? VotedAt { get; set; }

    public virtual Submission? Submission { get; set; }

    public virtual User? User { get; set; }
}
