using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Comment;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> CreateComment(CommentInfo request);
        Task<bool> DeleteComment(int id);
        Task<bool> UpdateComment(int id, CommentUpdateInfo request);
        Task<GetCommentResponse> GetCommentById(int id);
        Task<IPaginate<GetCommentResponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel);
    }
}
