using AutoMapper;
using Origami.BusinessTier.Payload.Vote;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class VoteModule : Profile
    {
        public VoteModule()
        {
            CreateMap<Vote, GetVoteResponse>();
            CreateMap<VoteInfo, Vote>()
                .ForMember(d => d.VoteId, o => o.Ignore())
                .ForMember(d => d.VotedAt, o => o.Ignore());
        }
    }
}