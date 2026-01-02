using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class SubmissionComment
{
    public int CommentId { get; set; }

    public int SubmissionId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public int? ParentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<SubmissionComment> InverseParent { get; set; } = new List<SubmissionComment>();

    public virtual SubmissionComment? Parent { get; set; }

    public virtual Submission Submission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
