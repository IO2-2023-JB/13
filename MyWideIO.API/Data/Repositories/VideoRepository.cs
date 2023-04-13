using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VideoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VideoModel?> GetVideoAsync(Guid id)
        {
            return await _dbContext.Videos.FindAsync(id);
        }

        public void RemoveVideo(VideoModel video)
        {
            _dbContext.Videos.Remove(video);
        }
    }

}
