using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LikeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(ViewerLike like)
        {
            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(ViewerLike like)
        {
            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<ViewerLike> likes)
        {
            _dbContext.Likes.RemoveRange(likes);
            await _dbContext.SaveChangesAsync();
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

        public async Task UpdateAsync(ViewerLike like)
        {
            _dbContext.Likes.Update(like);
            await _dbContext.SaveChangesAsync();
        }
    }
}
