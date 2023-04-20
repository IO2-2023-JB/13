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
            return await _videoRepository.PutVideoData(id, dto);
        }

        public async Task UploadVideoAsync(Guid id, Stream videoStream) // raczej Stream video
        {
            // w zaleznosci od enuma rocessingProgressDto 
            // np jak jest Uploading


            VideoModel video = await _videoRepository.GetVideoAsync(id); // nie user nowe trzeba zrobic wyjatki dla video
            //VideoModel video = await _videoRepository.GetVideoAsync(id) ?? throw new UserNotFoundException();
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

    }
}
