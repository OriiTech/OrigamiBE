using AutoMapper;
using Origami.BusinessTier.Payload.Origami;

namespace Origami.API.Mappers
{
    public class OrigamiModule : Profile
    {
        public OrigamiModule()
        {
            CreateMap<DataTier.Models.Origami, GetOrigamiResponse>();
            CreateMap<OrigamiInfo, DataTier.Models.Origami>();
        }
    }
}
