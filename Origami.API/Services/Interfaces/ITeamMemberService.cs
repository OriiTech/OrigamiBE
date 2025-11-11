using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TeamMember;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ITeamMemberService
    {
        Task<int> CreateNewTeamMember(TeamMemberInfo request);
        Task<IPaginate<GetTeamMemberResponse>> ViewAllTeamMembers(TeamMemberFilter filter, PagingModel pagingModel);
        Task<GetTeamMemberResponse> GetTeamMemberById(int id);
        Task<bool> UpdateTeamMember(int id, TeamMemberInfo request);
        Task<bool> DeleteTeamMember(int id);
    }
}