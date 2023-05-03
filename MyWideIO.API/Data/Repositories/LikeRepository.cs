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
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new CustomException("Adding to database error"); // nie wiem czy to jest potrzebne
        }

        public async Task DeleteAsync(ViewerLike like)
        {
            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ViewerLike?> GetUserLikeOfVideoAsync(Guid userId, Guid videoId)
        {
            return await _dbContext.Likes.Where(l => l.VideoId == videoId && l.ViewerId == userId).FirstOrDefaultAsync();   // nie moze byc kilka w bazie mam nadzieje
        }                                                                                                                           // jak moze byc to trzeba dodac klucz glowny na obu id (composite pk)

        public async Task<ICollection<ViewerLike>> GetUserLikesAsync(Guid userId)
        {
            return await _dbContext.Likes.Where(l => l.ViewerId == userId).ToListAsync();
        }

        public async Task<ICollection<ViewerLike>> GetVideoLikesAsync(Guid videoId)
        {
            return await _dbContext.Likes.Where(l => l.VideoId == videoId).ToListAsync();
        }

        public async Task UpdateAsync(ViewerLike like)
        {
            _dbContext.Likes.Update(like);
            await _dbContext.SaveChangesAsync();
        }
    }
}
