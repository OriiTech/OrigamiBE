using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Guide> Guides { get; set; } = new List<Guide>();
}
