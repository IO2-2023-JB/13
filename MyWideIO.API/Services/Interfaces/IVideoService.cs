using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.DB_Models;
using WideIO.API.Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoService
    {
        public Task<Stream> GetVideo(Guid id);
        public Task<bool> RemoveVideoIfExist(Guid id); // po co te boole
        public Task<bool> UpdateVideo(Guid id, VideoUploadDto dto);
        public Task<VideoUploadResponseDto> UploadVideoMetadata(VideoUploadDto dto, Guid creatorId);
        public Task UploadVideoAsync(Guid id, Stream videoFile);
        public Task UpdateVideoReaction(Guid videoId, Guid viewerId, VideoReactionUpdateDto videoReactionUpdateDto);
        public Task<VideoReactionDto> GetVideoReaction(Guid videoId, Guid viewerId);
        public Task<VideoListDto> GetUserVideosAsync(Guid id);
    }
}
