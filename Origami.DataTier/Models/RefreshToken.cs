using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string RefreshToken1 { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsActive { get; set; }
}
