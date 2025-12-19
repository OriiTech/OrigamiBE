using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class UserProfile
{
    public int UserId { get; set; }

    public string? DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
