using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models;
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

        public async Task<bool> UploadVideoAsync(Guid id, string video) // raczej Stream video
        {
            // w zaleznosci od enuma rocessingProgressDto 
            // np jak jest Uploading

            string fileName = id.ToString();
            BinaryData binaryData = BinaryData.FromString(video);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            Response response;
            response = (await blobClient.UploadAsync(binaryData, true)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Image upload error"); // image?
            }

            response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders())).GetRawResponse(); // ?
            return response.IsError ? throw new UserException("Video upload error") : true; // nigdy nie zwraca false, to po co Task<bool>
        }
        
        public async Task<VideoUploadResponseDto> UploadVideoMetadata(VideoUploadDto dto, Guid creatorId)
        {
            return await _videoRepository.UploadVideoMetadata(dto, creatorId);
        }

    }
}
