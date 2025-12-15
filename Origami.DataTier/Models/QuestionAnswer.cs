using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class QuestionAnswer
{
    public int AnswerId { get; set; }

    public int QuestionId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
