using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Comment;
using Origami.BusinessTier.Utils.EnumConvert;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    [ApiController]
    public class CommentController : BaseController<CommentController>
    {
        private readonly ICommentService _commentService;

        public CommentController(ILogger<CommentController> logger, ICommentService commentService)
            : base(logger)
        {
            _commentService = commentService;
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateComment(CommentInfo request)
        {
            var id = await _commentService.CreateComment(request);
            return Ok(new { CommentId = id });
        }

        [HttpGet(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(typeof(GetCommentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComment(int id)
        {
            var response = await _commentService.GetCommentById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Guide.GuideEndPoint + "/comments")]
        public async Task<IActionResult> GetComments([FromRoute] int id)
        {
            var response = await _commentService.GetCommentsByGuideId(id);
            return Ok(response);
        }



        [HttpGet(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetCommentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllComment([FromQuery] CommentFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _commentService.ViewAllComment(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPatch(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment(int id, CommentUpdateInfo request)
        {
            var isSuccessful = await _commentService.UpdateComment(id, request);
            if (!isSuccessful) return Ok("UpdateCommentFailed");
            return Ok("UpdateCommentSuccess");
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpDelete(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var isSuccessful = await _commentService.DeleteComment(id);
            if (!isSuccessful) return Ok("DeleteCommentFailed");
            return Ok("DeleteCommentSuccess");
        }
    }
}
