using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MyWideIO.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure;
using MyWideIO.API.Exceptions;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class AzureBlobVideoStorageService : IVideoStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string containerName = "video";
        public AzureBlobVideoStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
        }

        public async Task<Stream> GetVideoFileAsync(Guid id)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");

            return await blobClient.OpenReadAsync();
        }
        public async Task RemoveVideoFileAsync(Guid id)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");
            await blobClient.DeleteAsync();
        }

        public async Task UploadVideoFileAsync(Guid id, Stream stream)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");

            Response response;
            response = (await blobClient.UploadAsync(stream, true)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Video upload error");
            }
        }
    }
}
