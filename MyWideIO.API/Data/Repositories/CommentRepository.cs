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

        public async Task<List<CommentModel>> GetVideoComments(Guid videoId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Comments
                .Include(c => c.Author)
                .Where(c => c.VideoId == videoId)
                .Where(c => c.ParentCommentId == null)
                .ToListAsync(cancellationToken);

        }

        public async Task<List<CommentModel>> GetCommentResponses(Guid commentId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Comments
                .Include(c => c.Author)
                .Where(c => c.ParentCommentId == commentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CommentModel>> GetUserCommentsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Comments
                .Include(c => c.Author)
                .Where(c => c.AuthorId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<CommentModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Comments
                .Include(c => c.Author)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
