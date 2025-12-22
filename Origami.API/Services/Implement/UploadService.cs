using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Origami.API.Services.Interfaces;

namespace Origami.API.Services.Implement
{
    public class UploadService : IUploadService
    {
        private readonly string _bucket;
        private readonly StorageClient _storageClient;
        public UploadService(IConfiguration config)
        {
            _bucket = config["Firebase:Bucket"]
                ?? throw new Exception("Firebase Bucket not configured");

            var credentialPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                config["Firebase:CredentialPath"]!
            );

            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
        }

        public async Task<string> UploadAsync(IFormFile file, string? folder)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var objectName = string.IsNullOrEmpty(folder)
                ? fileName
                : $"{folder}/{fileName}";

            using var stream = file.OpenReadStream();

            await _storageClient.UploadObjectAsync(
                _bucket,
                objectName,
                file.ContentType,
                stream
            );

            return $"https://firebasestorage.googleapis.com/v0/b/{_bucket}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }
    }
}
