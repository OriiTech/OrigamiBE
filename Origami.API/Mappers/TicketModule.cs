using AutoMapper;
using Origami.BusinessTier.Payload.Ticket;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class TicketMapper : Profile
    {
        public TicketMapper()
        {
            CreateMap<Ticket, GetTicketResponse>();
            CreateMap<TicketInfo, Ticket>();

        }
    }
}
