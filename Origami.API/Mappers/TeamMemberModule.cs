using AutoMapper;
using Origami.BusinessTier.Payload.TeamMember;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class TeamMemberModule : Profile
    {
        public TeamMemberModule()
        {
            CreateMap<TeamMember, GetTeamMemberResponse>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.TeamName : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
            CreateMap<TeamMemberInfo, TeamMember>()
                .ForMember(dest => dest.TeamMemberId, opt => opt.Ignore())
                .ForMember(dest => dest.JoinedAt, opt => opt.Ignore());
        }
    }
}