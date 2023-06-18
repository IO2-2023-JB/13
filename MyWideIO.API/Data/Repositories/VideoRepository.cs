using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Data.Repositories
{
    public class VideoRepository : Repository<VideoModel>, IVideoRepository
    {

        public VideoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
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
                .Include(v => v.Tags)
                .Include(v => v.Creator)
                .ToListAsync(cancellationToken);
        }
        public async Task<VideoModel?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Videos
                .Include(v => v.Creator)
                .Include(v => v.Tags)
                .Where(v => v.Id == id)
                .SingleAsync(cancellationToken);
        }

        public async Task<bool> UserHasVideosAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Videos
                .Where(v => v.CreatorId == userId)
                .AnyAsync(cancellationToken);
        }

        public async Task<List<VideoModel>> GetUserReccomendationList(Guid userId, int n)
        {
            return await _dbContext.Videos
                .Include(v => v.Creator)
                .Include(v => v.Tags)
                .Where(v => v.IsVisible)
                .OrderBy(v => Guid.NewGuid())
                .Take(n)
                .ToListAsync();
        }

        public async Task<ICollection<VideoModel>> GetUploadingUploadedProcessingVideos()
        {
            return await _dbContext.Videos
                .Where(v =>
                v.ProcessingProgress == ProcessingProgressEnum.Uploading
                || v.ProcessingProgress == ProcessingProgressEnum.Processing
                || v.ProcessingProgress == ProcessingProgressEnum.Uploaded
                )
                .ToListAsync();
        }
    }

}
