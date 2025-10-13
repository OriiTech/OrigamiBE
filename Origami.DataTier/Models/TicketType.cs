using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class TicketType
{
    public int TicketTypeId { get; set; }

    public string? TicketTypeName { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
