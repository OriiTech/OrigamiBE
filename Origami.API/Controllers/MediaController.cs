using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;

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
    [RequestSizeLimit(50_000_000)] // 50MB
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        var url = await _cloudinaryService.UploadImageAsync(file);
        return Ok(new { url });
    }

    [HttpPost("api/v1/media/upload-video")]
    [RequestSizeLimit(200_000_000)] // 200MB
    public async Task<IActionResult> UploadVideo([FromForm] IFormFile file)
    {
        var url = await _cloudinaryService.UploadVideoAsync(file);
        return Ok(new { url });
    }
}


