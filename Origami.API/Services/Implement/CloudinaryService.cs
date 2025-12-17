using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Origami.API.Services.Interfaces;

namespace Origami.API.Services.Implement;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var section = configuration.GetSection("Cloudinary");
        var cloudName = section["CloudName"] ?? string.Empty;
        var apiKey = section["ApiKey"] ?? string.Empty;
        var apiSecret = section["ApiSecret"] ?? string.Empty;

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BadHttpRequestException("FileEmpty");

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "origami/images"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK)
            throw new BadHttpRequestException("UploadImageFailed");

        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadVideoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BadHttpRequestException("FileEmpty");

        await using var stream = file.OpenReadStream();
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "origami/videos"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK)
            throw new BadHttpRequestException("UploadVideoFailed");

        return result.SecureUrl.ToString();
    }
}


