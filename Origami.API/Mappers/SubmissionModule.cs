using AutoMapper;
using Origami.BusinessTier.Payload.Submission;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class SubmissionModule : Profile
    {
        public SubmissionModule()
        {
            CreateMap<Submission, GetSubmissionResponse>();
            CreateMap<SubmissionInfo, Submission>()
                .ForMember(d => d.SubmissionId, o => o.Ignore())
                .ForMember(d => d.SubmittedAt, o => o.Ignore());
        }
    }
}