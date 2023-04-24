using MyWideIO.API.Models.DB_Models;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoService
    {
        public Task<Stream> GetVideoAsync(Guid id);
        public Task RemoveVideoAsync(Guid id); // po co te boole
        public Task UpdateVideoAsync(Guid id, VideoUploadDto dto);
        public Task<VideoUploadResponseDto> UploadVideoMetadataAsync(VideoUploadDto dto, Guid creatorId);
        public Task UploadVideoAsync(Guid id, Stream videoFile);
        public Task UpdateVideoReactionAsync(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto);
        public Task<VideoReactionDto> GetVideoReactionAsync(Guid videoId, Guid viewerId);
        public Task<VideoMetadataDto> GetVideoMetadataAsync(Guid id);
        public Task<VideoListDto> GetUserVideosAsync(Guid id);
    }
}
