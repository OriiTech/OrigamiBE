using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int GuideId { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ParentId { get; set; }

    public virtual Guide Guide { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
