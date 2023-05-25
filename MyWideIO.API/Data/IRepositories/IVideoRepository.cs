using MyWideIO.API.Models;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using WideIO.API.Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IVideoRepository 
    {
        public Task<VideoModel?> GetVideoAsync(Guid id, CancellationToken cancellationToken = default);
        public Task RemoveVideoAsync(VideoModel video);
        public Task AddVideoAsync(VideoModel video);
        public Task UpdateVideoAsync(VideoModel video);
        public Task<ICollection<VideoModel>> GetUserVideosAsync(Guid id, CancellationToken cancellationToken = default);
        public IQueryable<VideoModel> GetIQuerableVideos();
    }
}
