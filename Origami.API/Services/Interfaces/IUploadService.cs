namespace Origami.API.Services.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadAsync(IFormFile file, string? folder = null);
        Task<string> GetSignedUrlAsync(string objectName, TimeSpan? expiration = null);
    }

}
