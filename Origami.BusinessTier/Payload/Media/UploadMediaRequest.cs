using Microsoft.AspNetCore.Http;

namespace Origami.BusinessTier.Payload.Media;

public class UploadMediaRequest
{
    public IFormFile File { get; set; } = default!;
}


