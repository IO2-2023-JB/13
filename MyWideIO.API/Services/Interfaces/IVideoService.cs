using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoService
    {
        public Task<Stream> GetVideoAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken = default);
        public Task RemoveVideoAsync(Guid videoId, Guid creatorId);
        public Task UpdateVideoAsync(Guid videoId, Guid creatorId, VideoUploadDto dto);
        public Task<VideoUploadResponseDto> UploadVideoMetadataAsync(VideoUploadDto dto, Guid creatorId);
        public Task UploadVideoAsync(Guid videoId, Guid creatorId, Stream videoFile, string extension, CancellationToken cancellationToken = default);
        public Task UpdateVideoReactionAsync(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto);
        public Task<VideoReactionDto> GetVideoReactionAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken = default);
        public Task<VideoMetadataDto> GetVideoMetadataAsync(Guid videoId, Guid viewerId, CancellationToken cancellationToken = default);
        public Task<VideoListDto> GetUserVideosAsync(Guid creatorId, Guid viewerId, CancellationToken cancellationToken = default);
        public Task<VideoListDto> GetVideosSubscribedByUser(Guid subscriber);
    }
}
