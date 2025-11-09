using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Ticket
{
    public class TicketInfo
    {
        public int UserId { get; set; }
        public int TicketTypeId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Status { get; set; }
    }
}
