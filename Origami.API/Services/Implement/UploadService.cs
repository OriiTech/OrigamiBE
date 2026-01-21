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
                ?? Environment.GetEnvironmentVariable("FIREBASE_BUCKET")
                ?? throw new Exception("Firebase Bucket not configured");

            GoogleCredential? credential = null;

            // Ưu tiên đọc từ environment variable (cho Render deployment)
            var firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
            if (!string.IsNullOrEmpty(firebaseCredentialsJson))
            {
                credential = GoogleCredential.FromJson(firebaseCredentialsJson);
            }
            else
            {
                // Fallback: đọc từ file (cho local development)
                var credentialPath = config["Firebase:CredentialPath"];
                if (string.IsNullOrEmpty(credentialPath))
                {
                    throw new InvalidOperationException(
                        "FIREBASE_CREDENTIALS_JSON environment variable is not set and Firebase:CredentialPath is not configured. " +
                        "Please set FIREBASE_CREDENTIALS_JSON on Render or configure Firebase:CredentialPath in appsettings.json for local development.");
                }

                var fullCredentialPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    credentialPath
                );

                if (!File.Exists(fullCredentialPath))
                {
                    throw new FileNotFoundException(
                        $"Firebase credentials file not found at: {fullCredentialPath}. " +
                        "Please set FIREBASE_CREDENTIALS_JSON environment variable on Render or ensure the file exists for local development.");
                }

                credential = GoogleCredential.FromFile(fullCredentialPath);
            }

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
