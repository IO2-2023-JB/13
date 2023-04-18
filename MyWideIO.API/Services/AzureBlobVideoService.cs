using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using System.ComponentModel;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class AzureBlobVideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string containerName = "video";

        public AzureBlobVideoService(BlobServiceClient blobServiceClient, IVideoRepository videoRepository)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
            _videoRepository = videoRepository;
        }

        public async Task<Stream> GetVideo(Guid id)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(id) ?? throw new UserNotFoundException(); // nie user nowe trzeba zrobic wyjatki dla video
            BlobClient blobClient = _blobContainerClient.GetBlobClient(video.fileName);

            return await blobClient.OpenReadAsync();
        }

        public async Task<bool> RemoveVideoIfExist(Guid id)
        {
            VideoModel? video = await _videoRepository.GetVideoAsync(id);
            if (video == null)
                return false;

            BlobClient blobClient = _blobContainerClient.GetBlobClient(video.fileName);
            blobClient.DeleteIfExists();
            _videoRepository.RemoveVideo(video);

            return true;
        }

        public async Task<bool> UpdateVideo(Guid id, VideoUploadDto dto)
        {
            return await _videoRepository.PutVideoData(id, dto);
        }
    }
}
