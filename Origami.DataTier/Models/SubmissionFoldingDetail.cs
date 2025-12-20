using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class SubmissionFoldingDetail
{
    public int SubmissionId { get; set; }

    public string? PaperSize { get; set; }

    public string? PaperType { get; set; }

    public int? Complexity { get; set; }

    public int? FoldingTimeMinute { get; set; }

    public string? Source { get; set; }

    public string? OriginalDesigner { get; set; }

    public virtual Submission Submission { get; set; } = null!;
}
