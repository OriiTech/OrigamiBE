using Microsoft.AspNetCore.Http;

namespace Origami.API.Services.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadVideoAsync(IFormFile file);
}


