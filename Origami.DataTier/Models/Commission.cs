using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Commission
{
    public int CommissionId { get; set; }

    public int? RevenueId { get; set; }

    public decimal? Percent { get; set; }

    public virtual Revenue? Revenue { get; set; }
}
