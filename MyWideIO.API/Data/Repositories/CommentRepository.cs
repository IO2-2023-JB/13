using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class CommentRepository : Repository<CommentModel>, ICommentRepository
    {

        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<CommentModel>> GetVideoComments(Guid videoId)
        {
            return await _dbContext.Comments
                .Where(c => c.VideoId == videoId)
                .ToListAsync();

        }

        public async Task<List<CommentModel>> GetCommentResponses(Guid commentId)
        {
            return await _dbContext.Comments
                .Where(c => c.ParentCommentId == commentId)
                .ToListAsync(); 
        }

        public async Task<List<CommentModel>> GetUserCommentsAsync(Guid userId)
        {
            return await _dbContext.Comments
                .Where(c => c.AuthorId == userId)
                .ToListAsync();
        }
    }
}
