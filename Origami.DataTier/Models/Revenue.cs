using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Revenue
{
    public int RevenueId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public int? GuideId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

    public virtual Course? Course { get; set; }

    public virtual Guide? Guide { get; set; }

    public virtual User? User { get; set; }
}
