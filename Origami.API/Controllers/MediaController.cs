using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Media;

namespace Origami.API.Controllers;

[ApiController]
[Authorize]
public class MediaController : BaseController<MediaController>
{
    private readonly ICloudinaryService _cloudinaryService;

    public MediaController(ILogger<MediaController> logger, ICloudinaryService cloudinaryService) : base(logger)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("api/v1/media/upload-image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // 50MB
    public async Task<IActionResult> UploadImage([FromForm] UploadMediaRequest request)
    {
        var url = await _cloudinaryService.UploadImageAsync(request.File);
        return Ok(new { url });
    }

    [HttpPost("api/v1/media/upload-video")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(200_000_000)] // 200MB
    public async Task<IActionResult> UploadVideo([FromForm] UploadMediaRequest request)
    {
        var url = await _cloudinaryService.UploadVideoAsync(request.File);
        return Ok(new { url });
    }
}


