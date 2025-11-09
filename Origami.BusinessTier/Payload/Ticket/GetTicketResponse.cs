using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Ticket
{
    public class GetTicketResponse
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int TicketTypeId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
