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
    }
}
