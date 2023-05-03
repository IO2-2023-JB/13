using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MyWideIO.API.BackgroundProcessing;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using NReco.VideoConverter;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IVideoStorageService _videoStorageService;
        private readonly ILikeRepository _likeRepository;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly ITransactionService _transactionService;
        private readonly IBackgroundTaskQueue<VideoProcessWorkItem> _backgroundTaskQueue;
        public VideoService(IVideoRepository videoRepository, IImageStorageService imageService, IVideoStorageService videoStorageService, ILikeRepository likeRepository, UserManager<AppUserModel> userManager, ITransactionService transactionService, IBackgroundTaskQueue<VideoProcessWorkItem> backgroundTaskQueue)
        {
            _videoRepository = videoRepository;
            _imageStorageService = imageService;
            _videoStorageService = videoStorageService;
            _likeRepository = likeRepository;
            _userManager = userManager;
            _transactionService = transactionService;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<Stream> GetVideoAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();

            if (video.ProcessingProgress != ProcessingProgressEnum.Ready)
                throw new VideoException("Video not avaible");


            video.ViewCount++;
            await _videoRepository.UpdateVideoAsync(video);

            return await _videoStorageService.GetVideoFileAsync(video.Id, cancellationToken);
        }

        public async Task RemoveVideoAsync(Guid videoId, Guid creatorId)
        {
            var video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();

            switch (video.ProcessingProgress)
            {
                case ProcessingProgressEnum.Uploading:
                    throw new VideoException("can't delete while uploading");
                case ProcessingProgressEnum.Uploaded:
                    throw new VideoException("can't delete while in queue for processing");
                case ProcessingProgressEnum.Processing:
                    throw new VideoException("can't delete while processing");
            }
            await _videoRepository.RemoveVideoAsync(video);

            if (video.ProcessingProgress == ProcessingProgressEnum.Ready)
                await _videoStorageService.RemoveVideoFileAsync(video.Id);

            if (video.Thumbnail is not null)
                await _imageStorageService.RemoveImageAsync(video.Thumbnail.FileName);

        }

        public async Task UpdateVideoAsync(Guid videoId, Guid creatorId, VideoUploadDto dto)
        {
            var video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();

            video.Description = dto.Description;
            video.Title = dto.Title;
            video.Tags = dto.Tags.Select(t => new TagModel
            {
                Content = t
            }).ToList();
            video.EditDate = DateTime.Now;
            video.IsVisible = dto.Visibility == VisibilityEnum.Public;
            if (dto.Thumbnail is not null)
            {
                string imagePrefix = @"base64,";
                if (dto.Thumbnail.Contains(imagePrefix))
                    dto.Thumbnail = dto.Thumbnail.Split(imagePrefix)[1];
                video.Thumbnail = await _imageStorageService.UploadImageAsync(dto.Thumbnail, video.Id.ToString());
                await _videoRepository.UpdateVideoAsync(video);
            }
            else if (video.Thumbnail is not null)
            {
                await _imageStorageService.RemoveImageAsync(video.Thumbnail.FileName);
                video.Thumbnail = null;
            }
            await _videoRepository.UpdateVideoAsync(video);
        }



        public async Task<VideoMetadataDto> GetVideoMetadataAsync(Guid videoId, Guid viewerId)
        {
            var video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();

            return VideoMapper.VideoModelToVideoMetadataDto(video);
        }

        public async Task UploadVideoAsync(Guid videoId, Guid creatorId, Stream videoFile, string extension, CancellationToken cancellationToken)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            try
            {                
                if (video.CreatorId != creatorId)
                    throw new ForbiddenException();

                switch (video.ProcessingProgress)
                {
                    //case ProcessingProgressEnum.Ready:
                    case ProcessingProgressEnum.FailedToUpload:
                    case ProcessingProgressEnum.MetadataRecordCreated:
                        video.ProcessingProgress = ProcessingProgressEnum.Uploading;
                        break;

                    case ProcessingProgressEnum.Uploading:
                        throw new VideoException("Already Uploading");

                    case ProcessingProgressEnum.Uploaded:
                        throw new VideoException("Video is waiting to be processed");

                    case ProcessingProgressEnum.Processing:
                        throw new VideoException("Video is being processed");

                    default: throw new Exception("unknown error"); //
                }
                await _videoRepository.UpdateVideoAsync(video);
                var workItem = new VideoProcessWorkItem(videoId, new MemoryStream(), extension);
                await videoFile.CopyToAsync(workItem.VideoFile,cancellationToken);
                workItem.VideoFile.Position = 0;
                video.ProcessingProgress = ProcessingProgressEnum.Uploaded;
                await _videoRepository.UpdateVideoAsync(video);
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(workItem);
                // dodajemy do kolejki (Channel), z ktorej czyta background processing service i pokolei przetwarza
                // background service ma ta sama instacje kolejki co ta tutaj (singleton)
            }
            catch
            {
                video.ProcessingProgress = ProcessingProgressEnum.FailedToUpload;
                await _videoRepository.UpdateVideoAsync(video);
                throw;
            }

        }

        public async Task<VideoUploadResponseDto> UploadVideoMetadataAsync(VideoUploadDto dto, Guid creatorId)
        {
            var creator = await _userManager.FindByIdAsync(creatorId.ToString()) ?? throw new UserNotFoundException();
            if (creator.Money is null)
                throw new UserException("Creator doesn't have required properties");
            VideoModel video;
            await _transactionService.BeginTransactionAsync();
            try
            {

                video = new VideoModel
                {
                    Description = dto.Description,
                    Title = dto.Title,
                    Tags = dto.Tags.Select(t => new TagModel
                    {
                        Content = t
                    }).ToList(),
                    ProcessingProgress = ProcessingProgressEnum.MetadataRecordCreated,
                    CreatorId = creatorId,
                    Creator = creator,
                    IsVisible = dto.Visibility == VisibilityEnum.Public
                };
                await _videoRepository.AddVideoAsync(video); // id potrzebne do zdjecia
                if (dto.Thumbnail is not null)
                {
                    string imagePrefix = @"base64,";
                    if (dto.Thumbnail.Contains(imagePrefix))
                        dto.Thumbnail = dto.Thumbnail.Split(imagePrefix)[1];
                    video.Thumbnail = await _imageStorageService.UploadImageAsync(dto.Thumbnail, video.Id.ToString());
                    await _videoRepository.UpdateVideoAsync(video);
                }
                creator.OwnedVideos.Add(video);
                await _userManager.UpdateAsync(creator);
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();
            return new VideoUploadResponseDto
            {
                Id = video.Id,
                ProcessingProgress = video.ProcessingProgress
            };
        }

        public async Task UpdateVideoReactionAsync(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();
            AppUserModel user = await _userManager.FindByIdAsync(viewerId.ToString());
            ViewerLike? like = await _likeRepository.GetUserLikeOfVideoAsync(viewerId, videoId); // jakos inaczej
            if (like is null)
            {
                like = new ViewerLike
                {
                    Viewer = user,
                    Video = video,
                    VideoId = videoId,
                    ViewerId = viewerId,
                    Reaction = ReactionEnum.None
                };
                await _likeRepository.AddAsync(like);
            }
            switch (like.Reaction)
            {
                case ReactionEnum.Positive:
                    video.PositiveReactions--;
                    break;
                case ReactionEnum.Negative:
                    video.NegativeReactions--;
                    break;
                case ReactionEnum.None:
                    break;
                default:
                    throw new VideoException("Unknown reaction");
            }
            like.Reaction = videoReactionUpdateDto.Value;
            switch (like.Reaction)
            {
                case ReactionEnum.Positive:
                    video.PositiveReactions++;
                    break;
                case ReactionEnum.Negative:
                    video.NegativeReactions++;
                    break;
                case ReactionEnum.None:
                    break;
                default:
                    throw new VideoException("Unknown reaction");
            }
            video.LikedBy.Add(like); // None usuwamy czy zostaje
            await _videoRepository.UpdateVideoAsync(video);
            await _likeRepository.UpdateAsync(like);
        }

        public async Task<VideoReactionDto> GetVideoReactionAsync(Guid videoId, Guid viewerId)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();
            ViewerLike? like = await _likeRepository.GetUserLikeOfVideoAsync(viewerId, videoId);
            return new VideoReactionDto
            {
                PositiveCount = video.PositiveReactions,
                NegativeCount = video.NegativeReactions,
                CurrentUserReaction = like?.Reaction ?? ReactionEnum.None
            };

        }

        public async Task<VideoListDto> GetUserVideosAsync(Guid creatorId, Guid viewerId)
        {
            var list = (await _videoRepository.GetUserVideosAsync(creatorId)).Select(m => VideoMapper.VideoModelToVideoMetadataDto(m));
            if (creatorId != viewerId)
                list = list.Where(v => v.Visibility == VisibilityEnum.Public);
            return new VideoListDto
            {
                Videos = list.ToList()
            };
        }
    }
}
