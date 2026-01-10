using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class SubmissionImage
{
    public int ImageId { get; set; }

    public int? SubmissionId { get; set; }

    public string? Url { get; set; }

    public string? Thumbnail { get; set; }

    public string? Note { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Submission? Submission { get; set; }
}
