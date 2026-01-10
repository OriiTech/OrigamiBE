using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class GuidePromoPhoto
{
    public int PhotoId { get; set; }

    public int GuideId { get; set; }

    public int DisplayOrder { get; set; }

    public string Url { get; set; } = null!;

    public virtual Guide Guide { get; set; } = null!;
}
