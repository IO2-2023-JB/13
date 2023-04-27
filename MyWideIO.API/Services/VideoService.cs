﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using System.Collections.Concurrent;
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

        public VideoService(IVideoRepository videoRepository, IImageStorageService imageService, IVideoStorageService videoStorageService, ILikeRepository likeRepository, UserManager<AppUserModel> userManager)
        {
            _videoRepository = videoRepository;
            _imageStorageService = imageService;
            _videoStorageService = videoStorageService;
            _likeRepository = likeRepository;
            _userManager = userManager;
        }

        public async Task<Stream> GetVideoAsync(Guid videoId, Guid viewerId)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new ForbiddenException();

            if (!new[] { ProcessingProgressEnum.Uploaded, ProcessingProgressEnum.Ready, ProcessingProgressEnum.Processing }.Contains(video.ProcessingProgress))
                throw new VideoException("Video not available");
            //video.ProcessingProgress = ProcessingProgressEnum.Processing;
            // moze proccessing znaczy w trakcie ogladania?
            // na pewno moze kilka osoba na raz ogladac
            // wiec jesli jedna ustawi proccessing i potem druga
            // i pierwsza skonczy przed druga i zmieni na ready/uploaded
            // to bedzie mozna usunac, ale druga wciaz pobiera
            // await _videoRepository.UpdateVideoAsync(video);

            //if (!scopedUsersVideos.Contains((videoId, viewerId)))
            //{
            video.ViewCount++; // jak sie bedzie przewijac video to sie bedzie zwiekszac
                               // przy kazdym requescie
            await _videoRepository.UpdateVideoAsync(video);
           
            //   scopedUsersVideos.Add((videoId, viewerId));
            // }
            return await _videoStorageService.GetVideoFileAsync(video.Id);
        }

        public async Task RemoveVideoAsync(Guid videoId, Guid creatorId)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();

            if (video.ProcessingProgress == ProcessingProgressEnum.Uploading)
                throw new VideoException("can't delete while uploading");
            else if (video.ProcessingProgress == ProcessingProgressEnum.Processing)
                throw new VideoException("can't delete while proccessing");
            await _videoRepository.RemoveVideoAsync(video); // najpierw usuwamy z bazy zeby nie mozna bylo czytac w trakcie usuwania

            if (new[] { ProcessingProgressEnum.Uploaded }.Contains(video.ProcessingProgress)) // co z ready, proccesing
                await _videoStorageService.RemoveVideoFileAsync(video.Id);

            if (video.Thumbnail is not null)
                await _imageStorageService.RemoveImageAsync(video.Thumbnail.FileName);

        }
        // private readonly HashSet<(Guid, Guid)> scopedUsersVideos = new();

        public async Task UpdateVideoAsync(Guid videoId, Guid creatorId, VideoUploadDto dto)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();

            video.Description = dto.Description;
            video.Title = dto.Title;
            video.Tags = dto.Tags.Select(t => new TagModel { Content = t }).ToList();
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
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new ForbiddenException();

            return VideoMapper.VideoModelToVideoMetadataDto(video);
        }

        public async Task UploadVideoAsync(Guid videoId, Guid creatorId, Stream videoFile) // raczej Stream video
        {
            // race condition ?

            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (video.CreatorId != creatorId)
                throw new ForbiddenException();

            switch (video.ProcessingProgress)
            {
                // case ProcessingProgressEnum.Uploaded:
                case ProcessingProgressEnum.FailedToUpload:
                case ProcessingProgressEnum.MetadataRecordCreated:
                    video.ProcessingProgress = ProcessingProgressEnum.Uploading;
                    break;

                case ProcessingProgressEnum.Uploading:
                    throw new VideoException("Already Uploading");

                case ProcessingProgressEnum.Uploaded: // nie mozna zmienic pliku na nowy?
                    return;

                //case ProcessingProgressEnum.FailedToUpload: //restart uploada
                //    video.ProcessingProgress = ProcessingProgressEnum.Uploading;
                //    break;
                default: throw new Exception("unknown error"); //todo  (co robi ready i processing dokładnie?)
            }
            await _videoRepository.UpdateVideoAsync(video);
            try
            {
                // TODO zmiana pliku na mp4

                await _videoStorageService.UploadVideoFileAsync(videoId, videoFile);

                video.ProcessingProgress = ProcessingProgressEnum.Uploaded;

                // response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders())).GetRawResponse();

            }
            catch
            {
                video.ProcessingProgress = ProcessingProgressEnum.FailedToUpload;
                throw;
            }
            finally
            {
                await _videoRepository.UpdateVideoAsync(video);
            }

        }

        public async Task<VideoUploadResponseDto> UploadVideoMetadataAsync(VideoUploadDto dto, Guid creatorId)
        {
            AppUserModel creator = await _userManager.FindByIdAsync(creatorId.ToString()) ?? throw new UserNotFoundException();
            if (creator.Money is null)
                throw new UserException("Creator doesn't have requered properites");
            var video = new VideoModel
            {
                Description = dto.Description,
                Title = dto.Title,
                Tags = dto.Tags.Select(t => new TagModel { Content = t }).ToList(),
                //Thumbnail = null,
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
            return new VideoUploadResponseDto
            {
                Id = video.Id,
                ProcessingProgress = video.ProcessingProgress
            };
        }

        public async Task UpdateVideoReactionAsync(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible)
                throw new ForbiddenException();
            AppUserModel user = await _userManager.FindByIdAsync(viewerId.ToString());
            ViewerLike? like = await _likeRepository.GetUserLikeOfVideoAsync(viewerId, videoId); // jakos inaczej
            like ??= new ViewerLike
            {
                Viewer = user,
                Video = video,
                VideoId = videoId,
                ViewerId = viewerId,
                Reaction = ReactionEnum.None

            };
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
            }
            video.LikedBy.Add(like); // None usuwamy czy zostaje
            await _videoRepository.UpdateVideoAsync(video);
            await _likeRepository.UpdateAsync(like);
        }

        public async Task<VideoReactionDto> GetVideoReactionAsync(Guid videoId, Guid viewerId)
        {
            VideoModel video = await _videoRepository.GetVideoAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible)
                throw new ForbiddenException();
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
