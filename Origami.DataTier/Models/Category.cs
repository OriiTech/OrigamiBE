using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Guide> Guides { get; set; } = new List<Guide>();
}
