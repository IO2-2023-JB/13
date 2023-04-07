using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace MyWideIO.API.Services
{
    public class AzureBlobImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string containerName = "blob1";

        public AzureBlobImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
        }

        public Task RemoveImageAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadImageAsync(string base64image, string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

            byte[] buffer = Convert.FromBase64String(base64image);

            BinaryData binaryData = new BinaryData(buffer);

            await blobClient.UploadAsync(binaryData, true);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = "image/png"
            });
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
