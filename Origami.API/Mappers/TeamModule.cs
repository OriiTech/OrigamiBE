using AutoMapper;
using Origami.BusinessTier.Payload.Team;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class TeamModule : Profile
    {
        public TeamModule()
        {
            CreateMap<Team, GetTeamResponse>()
                .ForMember(dest => dest.ChallengeTitle, opt => opt.MapFrom(src => src.Challenge != null ? src.Challenge.Title : null))
                .ForMember(dest => dest.MemberCount, opt => opt.Ignore()); // Set manually in service
            CreateMap<TeamInfo, Team>()
                .ForMember(dest => dest.TeamId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}