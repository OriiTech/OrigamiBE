using AutoMapper;
using Origami.BusinessTier.Payload.Score;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class ScoreModule : Profile
    {
        public ScoreModule()
        {
            CreateMap<Score, GetScoreResponse>()
                .ForMember(d => d.Score, o => o.MapFrom(s => s.Score1));
            CreateMap<ScoreInfo, Score>()
                .ForMember(d => d.ScoreId, o => o.Ignore())
                .ForMember(d => d.Score1, o => o.MapFrom(s => s.Score))
                .ForMember(d => d.ScoreAt, o => o.Ignore());
        }
    }
}