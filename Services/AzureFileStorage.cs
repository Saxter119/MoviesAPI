using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesAPI.Services
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string connectionString;

        public AzureFileStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task DeleteFile(string route, string container)
        {
            if (string.IsNullOrEmpty(route)) return;

            var client = new BlobContainerClient(connectionString, container);
            
            await client.CreateIfNotExistsAsync();

            var file = Path.GetFileName(route);

            var blob = client.GetBlobClient(file);

            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var client = new BlobContainerClient(connectionString, container);

            await client.CreateIfNotExistsAsync();

            client.SetAccessPolicy(PublicAccessType.Blob);

            string fileName = $"{Guid.NewGuid()}{extension}";

            var blob = client.GetBlobClient(fileName);

            BlobUploadOptions blobUploadOptions = new BlobUploadOptions();

            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();

            blobHttpHeaders.ContentType = contentType;

            blobUploadOptions.HttpHeaders = blobHttpHeaders;

            await blob.UploadAsync(new BinaryData(content), blobUploadOptions);

            return blob.Uri.ToString();
        }
    }
}
