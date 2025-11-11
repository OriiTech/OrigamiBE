using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Team;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ITeamService
    {
        Task<int> CreateNewTeam(TeamInfo request);
        Task<IPaginate<GetTeamResponse>> ViewAllTeams(TeamFilter filter, PagingModel pagingModel);
        Task<GetTeamResponse> GetTeamById(int id);
        Task<bool> UpdateTeamInfo(int id, TeamInfo request);
        Task<bool> DeleteTeam(int id);
    }
}