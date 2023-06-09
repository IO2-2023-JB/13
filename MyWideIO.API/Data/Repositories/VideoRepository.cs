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
        public override async Task<VideoModel?> GetAsync(Guid id, CancellationToken cancellationToken)
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

    }

}
