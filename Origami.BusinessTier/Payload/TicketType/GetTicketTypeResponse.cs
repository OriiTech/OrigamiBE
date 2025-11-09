using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.TicketType
{
    public class GetTicketTypeResponse
    {
        public int TicketTypeId { get; set; }
        public string? TicketTypeName { get; set; }
    }
}
