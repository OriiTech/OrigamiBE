using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class CourseAccess
{
    public int AccessId { get; set; }

    public int? CourseId { get; set; }

    public int? LearnerId { get; set; }

    public DateTime? PurchasedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Learner { get; set; }
}
