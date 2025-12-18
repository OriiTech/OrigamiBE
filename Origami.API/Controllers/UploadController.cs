using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;

namespace Origami.API.Controllers
{
    [ApiController]
    [Route(ApiEndPointConstant.ApiEndpoint + "/uploads")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _storage;

        public UploadController(IUploadService storage)
        {
            _storage = storage;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            var url = await _storage.UploadAsync(request.File, request.Folder);
            return Ok(new { url });
        }

    }

}
