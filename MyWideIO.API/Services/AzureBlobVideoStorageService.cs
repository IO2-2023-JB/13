using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Exceptions;
using NReco.VideoConverter;

namespace MyWideIO.API.Services
{
    public class AzureBlobVideoStorageService : IVideoStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private const string ContainerName = "video";
        public AzureBlobVideoStorageService(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = blobServiceClient.CreateBlobContainer(ContainerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
        }

        public async Task<Stream> GetVideoFileAsync(Guid id, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");


            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }
        public async Task RemoveVideoFileAsync(Guid id)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");
            await blobClient.DeleteAsync();
        }

        public async Task UploadVideoFileAsync(Guid id, Stream stream, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");
            var response = (await blobClient.UploadAsync(stream, true, cancellationToken)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Video upload error");
            }
        }
        public Task<int> GetVideoDurationAsync(Guid id)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");
            BlobProperties p = blobClient.GetProperties();
            return Task.FromResult(0);
        }
        
    }
}
