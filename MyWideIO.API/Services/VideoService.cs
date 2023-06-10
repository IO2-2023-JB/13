using Microsoft.AspNetCore.Identity;
using MyWideIO.API.BackgroundProcessing;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using NReco.VideoConverter;
using System.Net.Sockets;

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
        private readonly ICommentRepository _commentRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public VideoService(
            IVideoRepository videoRepository,
            IImageStorageService imageService,
            IVideoStorageService videoStorageService,
            ILikeRepository likeRepository,
            UserManager<AppUserModel> userManager,
            ITransactionService transactionService,
            IBackgroundTaskQueue<VideoProcessWorkItem> backgroundTaskQueue,
            ICommentRepository commentRepository,
            IPlaylistRepository playlistRepository,
            ITicketRepository ticketRepository,
            ISubscriptionRepository subscriptionRepository
            )
        {
            _videoRepository = videoRepository;
            _imageStorageService = imageService;
            _videoStorageService = videoStorageService;
            _likeRepository = likeRepository;
            _userManager = userManager;
            _transactionService = transactionService;
            _backgroundTaskQueue = backgroundTaskQueue;
            _commentRepository = commentRepository;
            _playlistRepository = playlistRepository;
            _ticketRepository = ticketRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Stream> GetVideoAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken)
        {
            VideoModel video = await _videoRepository.GetAsync(videoId, cancellationToken) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();

            if (video.ProcessingProgress != ProcessingProgressEnum.Ready)
                throw new VideoException("Video not available");


            video.ViewCount++;
            await _videoRepository.UpdateAsync(video);

            return await _videoStorageService.GetVideoFileAsync(video.Id, cancellationToken);
        }

        public async Task RemoveVideoAsync(Guid videoId, Guid creatorId)
        {
            var video = await _videoRepository.GetAsync(videoId) ?? throw new VideoNotFoundException();
            var user = await _userManager.FindByIdAsync(creatorId.ToString()) ?? throw new UserNotFoundException();
            if (video.CreatorId != creatorId && !await _userManager.IsInRoleAsync(user, UserTypeEnum.Administrator.ToString()))
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
            // delete likes
            var likes = await _likeRepository.GetVideoLikesAsync(videoId);
            await _likeRepository.RemoveAsync(likes);

            // delete comments and responses
            var comments = await _commentRepository.GetVideoComments(videoId);
            await _commentRepository.RemoveAsync(comments);

            // remove video from playlists
            var playlists = await _playlistRepository.GetPlaylistsContainingVideo(videoId, true);
            foreach (var playlist in playlists)
            {
                var videoplaylist = playlist.VideoPlaylists.First(vp => vp.VideoId == videoId);
                playlist.VideoPlaylists.Remove(videoplaylist);
                await _playlistRepository.UpdateAsync(playlist);
            }

            // remove tickets?
            //if (await _ticketRepository.TargetHasTickets(videoId))
            //    throw new TicketException("Can't remove video while there are tickets for it");

            var tickets = await _ticketRepository.GetTargetsTickets(videoId);
            await _ticketRepository.RemoveAsync(tickets);


            // remove video
            await _videoRepository.RemoveAsync(video);




            if (video.ProcessingProgress == ProcessingProgressEnum.Ready)
                await _videoStorageService.RemoveVideoFileAsync(video.Id);

            if (video.Thumbnail is not null)
                await _imageStorageService.RemoveImageAsync(video.Thumbnail.FileName);

        }
        public async Task UpdateVideoAsync(Guid videoId, Guid creatorId, VideoUploadDto dto)
        {
            var video = await _videoRepository.GetAsync(videoId) ?? throw new VideoNotFoundException();
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
            }
            else if (video.Thumbnail is not null)
            {
                if (video.ProcessingProgress is ProcessingProgressEnum.Ready)
                {
                    Stream thumbnail = await GetThumbnailFromUploadedVideo(video.Id);
                    video.Thumbnail = await _imageStorageService.UploadImageAsync(thumbnail, video.Id.ToString());
                }
                else
                {
                    await _imageStorageService.RemoveImageAsync(video.Thumbnail.FileName);
                    video.Thumbnail = null;
                }
            }
            await _videoRepository.UpdateAsync(video);
        }
        private async Task<Stream> GetThumbnailFromUploadedVideo(Guid videoId)
        {
            Stream video = await _videoStorageService.GetVideoFileAsync(videoId);
            var ffmpeg = new FFMpegConverter();
            var outputThumbnailStream = new MemoryStream();

            const int byteLimit = 1024 * 1024; // 1MB

            byte[] buffer = new byte[byteLimit];

            int bytesRead = await video.ReadAsync(buffer.AsMemory(0, byteLimit));

            if (bytesRead == 0)
                throw new VideoException("While trying to extract thumbnail, the video was empty or could not be read.");

            string tempFilePath = Path.GetTempFileName();
            await File.WriteAllBytesAsync(tempFilePath, buffer);

            ffmpeg.GetVideoThumbnail(tempFilePath, outputThumbnailStream);
            File.Delete(tempFilePath);
            outputThumbnailStream.Position = 0;
            return outputThumbnailStream;
        }


        public async Task<VideoMetadataDto> GetVideoMetadataAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.GetAsync(videoId, cancellationToken) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();

            return video.ToVideoMetadataDto();
        }

        public async Task UploadVideoAsync(Guid videoId, Guid creatorId, Stream videoFile, string extension, CancellationToken cancellationToken)
        {
            VideoModel video = await _videoRepository.GetAsync(videoId, cancellationToken) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();
            try
            {
                video.ProcessingProgress = video.ProcessingProgress switch
                {
                    // ProcessingProgressEnum.Ready or
                    ProcessingProgressEnum.FailedToUpload or
                    ProcessingProgressEnum.FailedToProcess or
                    ProcessingProgressEnum.MetadataRecordCreated => ProcessingProgressEnum.Uploading,
                    ProcessingProgressEnum.Uploading => throw new VideoException("Already Uploading"),
                    ProcessingProgressEnum.Uploaded => throw new VideoException("Video is waiting to be processed"),
                    ProcessingProgressEnum.Processing => throw new VideoException("Video is being processed"),
                    _ => throw new Exception("unknown error")
                };
                await _videoRepository.UpdateAsync(video);
                var workItem = new VideoProcessWorkItem(videoId, Path.GetTempFileName());
                using (var filestream = File.OpenWrite(workItem.fileName))
                {
                    await videoFile.CopyToAsync(filestream, cancellationToken);
                }
                video.ProcessingProgress = ProcessingProgressEnum.Uploaded;
                await _videoRepository.UpdateAsync(video);
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(workItem);
            }
            catch
            {
                video.ProcessingProgress = ProcessingProgressEnum.FailedToUpload;
                await _videoRepository.UpdateAsync(video);
                throw;
            }

        }

        public async Task<VideoUploadResponseDto> UploadVideoMetadataAsync(VideoUploadDto dto, Guid creatorId)
        {
            var creator = await _userManager.FindByIdAsync(creatorId.ToString()) ?? throw new UserNotFoundException();
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
                await _videoRepository.AddAsync(video); // id potrzebne do zdjecia
                creator.OwnedVideos.Add(video);
                await _userManager.UpdateAsync(creator);
                if (dto.Thumbnail is not null)
                {
                    string imagePrefix = @"base64,";
                    if (dto.Thumbnail.Contains(imagePrefix))
                        dto.Thumbnail = dto.Thumbnail.Split(imagePrefix)[1];
                    video.Thumbnail = await _imageStorageService.UploadImageAsync(dto.Thumbnail, video.Id.ToString());
                    await _videoRepository.UpdateAsync(video);
                }
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
            VideoModel video = await _videoRepository.GetAsync(videoId) ?? throw new VideoNotFoundException();
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
            else
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
            await _videoRepository.UpdateAsync(video);
            await _likeRepository.UpdateAsync(like);
        }

        public async Task<VideoReactionDto> GetVideoReactionAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken)
        {
            VideoModel video = await _videoRepository.GetAsync(videoId, cancellationToken) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new VideoIsPrivateException();
            ViewerLike? like = await _likeRepository.GetUserLikeOfVideoAsync(viewerId, videoId, cancellationToken);
            return new VideoReactionDto
            {
                PositiveCount = video.PositiveReactions,
                NegativeCount = video.NegativeReactions,
                CurrentUserReaction = like?.Reaction ?? ReactionEnum.None
            };

        }

        public async Task<VideoListDto> GetUserVideosAsync(Guid creatorId, Guid viewerId, CancellationToken cancellationToken)
        {
            IEnumerable<VideoModel> list = await _videoRepository.GetUserVideosAsync(creatorId, cancellationToken);
            if (creatorId != viewerId)
                list = list.Where(v => v.IsVisible);
            return new VideoListDto
            {
                Videos = list.Select(v => v.ToVideoMetadataDto()).ToList(),
            };
        }

        public async Task<VideoListDto> GetVideosSubscribedByUser(Guid subscriberId)
        {
            var subscriptions = await _subscriptionRepository.GetViewersSubscriptionsAsync(subscriberId, true);

            return new VideoListDto
            {
                Videos = subscriptions.SelectMany(s => s.Creator.OwnedVideos.Where(v => v.IsVisible).Select(v => v.ToVideoMetadataDto())).ToList()
            };
        }
    }
}
