using AutoMapper;
using Origami.BusinessTier.Payload.Leaderboard;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class LeaderboardModule : Profile
    {
        public LeaderboardModule()
        {
            CreateMap<Leaderboard, GetLeaderboardResponse>();
            CreateMap<LeaderboardInfo, Leaderboard>()
                .ForMember(d => d.LeaderboardId, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}