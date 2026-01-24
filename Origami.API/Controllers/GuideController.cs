using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.BusinessTier.Utils.EnumConvert;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    [ApiController]
    public class GuideController : BaseController<GuideController>
    {
        private readonly IGuideService _guideService;

        public GuideController(ILogger<GuideController> logger, IGuideService guideService) : base(logger)
        {
            _guideService = guideService;
        }

        [HttpGet(ApiEndPointConstant.Guide.GuideEndPoint)]
        [ProducesResponseType(typeof(GetGuideResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGuide(int id)
        {
            var response = await _guideService.GetGuideById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Guide.GuideDetailEndPoint)]
        [ProducesResponseType(typeof(GetGuideDetailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGuideDetail(int id)
        {
            var response = await _guideService.GetGuideDetailById(id);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Guide.GuideViewEndPoint)]
        [Authorize(Roles = RoleConstants.User)]
        public async Task<IActionResult> IncreaseGuideView(int id)
        {
            await _guideService.IncreaseView(id);
            return NoContent();
        }

        [HttpGet(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGuide([FromQuery] GuideFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewAllGuide(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Guide.GuideCardsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideCardResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGuideCard([FromQuery] GuideCardFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewAllGuideCard(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpGet(ApiEndPointConstant.Guide.MyGuideCardsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideCardResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewMyGuideCards([FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewMyGuideCards(pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpGet(ApiEndPointConstant.Guide.MyFavoriteGuideCardsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideCardResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewMyFavoriteGuideCards([FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewMyFavoriteGuideCards(pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpGet(ApiEndPointConstant.Guide.MyPurchasedGuideCardsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideCardResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewMyPurchasedGuideCards([FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewMyPurchasedGuideCards(pagingModel);
            return Ok(response);
        }
        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGuide([FromBody] GuideSaveRequest request)
        {
            var id = await _guideService.CreateGuideAsync(request);     
            return Ok(new { guideId = id });
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPatch(ApiEndPointConstant.Guide.GuideEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGuideInfo(int id, [FromBody] GuideInfo request)
        {
            var isSuccessful = await _guideService.UpdateGuideInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Guide.GuidePromoPhotoEndPoint)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddPromoPhoto(int id, [FromForm] AddPromoPhotoRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("AddPromoPhoto: Request body is null");
                    return BadRequest(new { message = "Request body is required" });
                }

                if (request.PhotoFile == null || request.PhotoFile.Length == 0)
                {
                    _logger.LogWarning("AddPromoPhoto: PhotoFile is null or empty");
                    return BadRequest(new { message = "Photo file is required" });
                }

                _logger.LogInformation($"AddPromoPhoto: Processing request for guide {id}");
                var photoId = await _guideService.AddPromoPhotoAsync(id, request);
                _logger.LogInformation($"AddPromoPhoto: Successfully added promo photo {photoId} for guide {id}");
                return Ok(new { photoId });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning($"AddPromoPhoto: BadRequest - {ex.Message}");
                if (ex.Message.Contains("permission") || ex.Message.Contains("Unauthorized"))
                {
                    return StatusCode(403, new { message = ex.Message });
                }
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddPromoPhoto: Error adding promo photo for guide {id}");
                return StatusCode(500, new { message = "An error occurred while adding promo photo", error = ex.Message });
            }
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPut(ApiEndPointConstant.Guide.GuidePromoPhotoUpdateEndPoint)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdatePromoPhoto(int id, int photoId, [FromForm] UpdatePromoPhotoRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("UpdatePromoPhoto: Request body is null");
                    return BadRequest(new { message = "Request body is required" });
                }

                _logger.LogInformation($"UpdatePromoPhoto: Processing request for guide {id}, photo {photoId}");
                var isSuccessful = await _guideService.UpdatePromoPhotoAsync(id, photoId, request);
                if (!isSuccessful)
                {
                    return BadRequest(new { message = "Failed to update promo photo" });
                }
                _logger.LogInformation($"UpdatePromoPhoto: Successfully updated promo photo {photoId} for guide {id}");
                return Ok(new { message = "Promo photo updated successfully" });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning($"UpdatePromoPhoto: BadRequest - {ex.Message}");
                if (ex.Message.Contains("permission") || ex.Message.Contains("Unauthorized"))
                {
                    return StatusCode(403, new { message = ex.Message });
                }
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdatePromoPhoto: Error updating promo photo {photoId} for guide {id}");
                return StatusCode(500, new { message = "An error occurred while updating promo photo", error = ex.Message });
            }
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Guide.GuidePurchaseEndPoint)]
        [ProducesResponseType(typeof(PaymentGuideResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PurchaseGuide(int id, [FromBody] PaymentGuideRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                // Set guideId from route parameter
                request.GuideId = id;

                _logger.LogInformation($"PurchaseGuide: Processing purchase request for guide {id}");
                var response = await _guideService.PurchaseGuideAsync(request);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning($"PurchaseGuide: BadRequest - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PurchaseGuide: Error processing purchase for guide {request?.GuideId}");
                return StatusCode(500, new { message = "An error occurred while processing payment", error = ex.Message });
            }
        }

    }
}
