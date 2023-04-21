using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
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
        private readonly IImageService _imageService;

        public AzureBlobVideoService(BlobServiceClient blobServiceClient, IVideoRepository videoRepository, IImageService imageService)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
            _videoRepository = videoRepository;
            _imageService = imageService;
        }

        public async Task<Stream> GetVideo(Guid id)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(id) ?? throw new VideoNotFoundException();
            BlobClient blobClient = _blobContainerClient.GetBlobClient(video.Id.ToString() + ".mp4");

            return await blobClient.OpenReadAsync();
        }

        public async Task<bool> RemoveVideoIfExist(Guid id)
        {
            VideoModel? video = await _videoRepository.GetVideoAsync(id);
            if (video == null)
                return false;

            BlobClient blobClient = _blobContainerClient.GetBlobClient(video.Id.ToString() + ".mp4");
            blobClient.DeleteIfExists();
            _videoRepository.RemoveVideo(video);

            return true;
        }

        public async Task<bool> UpdateVideo(Guid id, VideoUploadDto dto)
        {
            return await _videoRepository.PutVideoData(id, dto, _imageService);
        }



        public async Task<VideoMetadataDto> GetVideoMetadata(Guid id)
        {
            VideoModel? model = await _videoRepository.GetVideoAsync(id);
            if (model == null)
                throw new ArgumentException("No video of given id");

            List<string> tgs = new List<string>();
            if(model.Tags != null)
                foreach (var t in model.Tags) 
                    tgs.Add(t.Content);

            return new VideoMetadataDto()
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                Thumbnail = _imageService.GetImageBase64("thumbnail_" + id.ToString() + ".png"),
                AuthorId = model.CreatorId,
                AuthorNickname = "TEST", // test
                ViewCount = 1, // test
                Tags = tgs,
                Visibility = model.IsVisible ? VisibilityDto.PublicEnum : VisibilityDto.PrivateEnum,
                ProcessingProgress = model.ProcessingProgress,
                EditDate = DateTime.Now, // test
                Duration = model.Duration.ToString()
            };
        }

        public async Task UploadVideoAsync(Guid id, Stream videoStream) // raczej Stream video
        {
            // w zaleznosci od enuma rocessingProgressDto 
            // np jak jest Uploading


            VideoModel video = await _videoRepository.GetVideoAsync(id) ?? throw new UserNotFoundException(); // nie user nowe trzeba zrobic wyjatki dla video
            
            switch (video.ProcessingProgress)
            {
                case ProcessingProgressDto.MetadataRecordCreated:
                    await _videoRepository.ModifyProcessingState(id, ProcessingProgressDto.UploadingEnum);
                    break;

                case ProcessingProgressDto.UploadingEnum:
                    throw new ApplicationException("Already Uploading");

                case ProcessingProgressDto.UploadedEnum: 
                    return;

                case ProcessingProgressDto.FailedToUploadEnum: //restart uploada
                    await _videoRepository.ModifyProcessingState(id, ProcessingProgressDto.UploadingEnum);
                    break;

                default: throw new Exception("unknown error"); //todo  (co robi ready i processing dokładnie?)
                    
            }

            /*
            //TODO zmiana pliku na mp4
            */

            BlobClient blobClient = _blobContainerClient.GetBlobClient(id.ToString() + ".mp4");

            Response response;
            response = (await blobClient.UploadAsync(videoStream, true)).GetRawResponse();
            if (response.IsError)
            {
                await _videoRepository.ModifyProcessingState(id, ProcessingProgressDto.FailedToUploadEnum);
                throw new UserException("Video upload error"); 
            }
            await _videoRepository.ModifyProcessingState(id, ProcessingProgressDto.UploadedEnum);
            return;
            //response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders())).GetRawResponse(); // ?
        }

        public async Task<VideoUploadResponseDto> UploadVideoMetadata(VideoUploadDto dto, Guid creatorId)
        {
            return await _videoRepository.UploadVideoMetadata(dto, creatorId);
        }

        public async Task UpdateVideoReaction(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto)
        {
            await _videoRepository.UpdateVideoReaction(videoId, viewerId, videoReactionUpdateDto);
        }

        public async Task<VideoReactionDto> GetVideoReaction(Guid videoId, Guid viewerId)
        {
            return await _videoRepository.GetVideoReaction(videoId, viewerId);
        }
    }
}
