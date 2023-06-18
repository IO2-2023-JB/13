using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Data.Repositories
{
    public class LikeRepository : Repository<ViewerLike>, ILikeRepository
    {

        public LikeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ViewerLike?> GetUserLikeOfVideoAsync(Guid userId, Guid videoId, CancellationToken cancellationToken)
        {
            return await _dbContext.Likes
                .Where(l => l.VideoId == videoId && l.ViewerId == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ICollection<ViewerLike>> GetUserLikesAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Likes
                .Where(l => l.ViewerId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<ViewerLike>> GetVideoLikesAsync(Guid videoId, CancellationToken cancellationToken)
        {
            return await _dbContext.Likes
                .Where(l => l.VideoId == videoId)
                .ToListAsync(cancellationToken);
        }
    }
}
