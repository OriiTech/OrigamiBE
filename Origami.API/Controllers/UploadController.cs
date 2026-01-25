using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using System.IO;
using System.Linq;

namespace Origami.API.Controllers
{
    [ApiController]
    [Route(ApiEndPointConstant.ApiEndpoint + "/uploads")]
    public class UploadController : BaseController<UploadController>
    {
        private readonly IUploadService _storage;

        public UploadController(
            ILogger<UploadController> logger,
            IUploadService storage) : base(logger)
        {
            _storage = storage;
        }

        /// <summary>
        /// Upload file ảnh lên Firebase Storage
        /// </summary>
        /// <param name="request">File và folder (optional, mặc định là root)</param>
        /// <returns>URL của file đã upload</returns>
        [HttpPost]
        [Authorize(Roles = RoleConstants.User)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "File is required" });
            }

            // Validate file type (image + video)
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var videoExtensions = new[] { ".mp4", ".mov", ".webm" };
            var allowedExtensions = imageExtensions.Concat(videoExtensions).ToArray();
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Chỉ chấp nhận ảnh (jpg, png, gif, webp) hoặc video (mp4, mov, webm)" });
            }

            // Validate file size: ảnh max 5MB, video max 50MB
            var maxFileSize = videoExtensions.Contains(fileExtension)
                ? 50L * 1024 * 1024   // 50MB cho video
                : 5L * 1024 * 1024;  // 5MB cho ảnh
            if (request.File.Length > maxFileSize)
            {
                return BadRequest(new { message = $"Kích thước file tối đa: {(maxFileSize / (1024 * 1024))}MB" });
            }

            try
            {
                var url = await _storage.UploadAsync(request.File, request.Folder);
                _logger.LogInformation($"File uploaded successfully: {url}");
                return Ok(new UploadResponse { Url = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, new { message = "Failed to upload file", error = ex.Message });
            }
        }
    }
}
