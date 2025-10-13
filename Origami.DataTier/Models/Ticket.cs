using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? UserId { get; set; }

    public int? TicketTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public virtual TicketType? TicketType { get; set; }

    public virtual User? User { get; set; }
}
