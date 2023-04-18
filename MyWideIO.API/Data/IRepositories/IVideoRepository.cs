using MyWideIO.API.Models.DB_Models;
using WideIO.API.Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IVideoRepository
    {
        public Task<VideoModel?> GetVideoAsync(Guid id);
        public void RemoveVideo(VideoModel video);
        public Task<bool> PutVideoData(Guid id, VideoUploadDto videoData);
    }
}
