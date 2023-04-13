using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IVideoRepository
    {
        public Task<VideoModel?> GetVideoAsync(Guid id);
        public void RemoveVideo(VideoModel video);
    }
}
