using AutoMapper;
using Origami.BusinessTier.Payload.TicketType;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class TicketTypeModule : Profile
    {
        public TicketTypeModule()
        {
            CreateMap<TicketType, GetTicketTypeResponse>().ReverseMap();
            CreateMap<TicketTypeInfo, TicketType>().ReverseMap();
        }
    }
}
