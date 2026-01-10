namespace Origami.API.Services.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadAsync(IFormFile file, string? folder = null);
    }

}
