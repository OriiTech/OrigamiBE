using AutoMapper;
using Origami.BusinessTier.Payload.Step;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class StepModule : Profile
    {
        public StepModule()
        {
            CreateMap<Step, StepInfo>();
            CreateMap<Step, GetStepResponse>();
            CreateMap<StepInfo, Step>()
                .ForMember(dest => dest.StepId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
