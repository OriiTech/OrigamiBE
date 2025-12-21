using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Origami.API.Services.Interfaces;

namespace Origami.API.Services.Implement
{
    public class UploadService : IUploadService
    {
        private readonly string? _bucket;
        private readonly StorageClient? _storageClient;
        private readonly string? _credentialPath;
        
        public UploadService(IConfiguration config)
        {
            _bucket = config["Firebase:Bucket"];
            _credentialPath = config["Firebase:CredentialPath"];

            // Only initialize StorageClient if credential file exists
            if (!string.IsNullOrEmpty(_credentialPath))
            {
                var credentialPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    _credentialPath
                );

                if (File.Exists(credentialPath))
                {
                    try
                    {
                        var credential = GoogleCredential.FromFile(credentialPath);
                        _storageClient = StorageClient.Create(credential);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't throw - allow app to start without Firebase
                        Console.WriteLine($"Warning: Failed to initialize Firebase StorageClient: {ex.Message}");
                    }
                }
            }
        }

        public async Task<string> UploadAsync(IFormFile file, string? folder)
        {
            if (string.IsNullOrEmpty(_bucket))
                throw new InvalidOperationException("Firebase Bucket not configured");

            if (_storageClient == null)
                throw new InvalidOperationException($"Firebase StorageClient not initialized. Please ensure Firebase credential file exists at: {_credentialPath}");

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
