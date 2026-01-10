using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class SubmissionLike
{
    public int SubmissionId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Submission Submission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
