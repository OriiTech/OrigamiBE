using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int? CourseId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Course? Course { get; set; }

    public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

    public virtual User? User { get; set; }
}
