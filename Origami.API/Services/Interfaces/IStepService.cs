using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Step;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IStepService
    {
        Task<int> CreateStep(StepInfo request);
        Task<bool> DeleteStep(int id);
        Task<GetStepResponse> GetStepById(int id);
        Task<bool> UpdateStepInfo(int id, StepInfo request);
        Task<IPaginate<GetStepResponse>> ViewAllSteps(StepFilter filter, PagingModel pagingModel);
    }
}
