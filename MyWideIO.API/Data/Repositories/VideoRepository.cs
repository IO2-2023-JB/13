using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Extensions;
using MyWideIO.API.Models;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VideoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddVideoAsync(VideoModel video)
        {
            _dbContext.Videos.Add(video);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<VideoModel> GetIQuerableVideos()
        {
            return _dbContext.Videos
                .Include(v => v.Tags)
                .Include(v => v.Creator)
                .AsNoTracking();
        }

        public async Task<ICollection<VideoModel>> GetUserVideosAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Videos
                .Where(v => v.CreatorId == id)
                .Include(v=>v.Tags)
                .Include(v=>v.Creator)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task<VideoModel?> GetVideoAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Videos
                .Include(v=>v.Creator)
                .Include(v=>v.Tags)
                .Where(v => v.Id == id)
                .SingleAsync(cancellationToken);
        }

        public async Task RemoveVideoAsync(VideoModel video)
        {
            _dbContext.Videos.Remove(video);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateVideoAsync(VideoModel video)
        {
            _dbContext.Videos.Update(video);
            await _dbContext.SaveChangesAsync();
        }
    }

}
