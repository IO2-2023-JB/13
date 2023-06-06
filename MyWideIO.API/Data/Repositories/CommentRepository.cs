using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddComment(CommentModel model)
        {
            _dbContext.Comments.Add(model);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<CommentModel?> GetComment(Guid commentId)
        {
            return await _dbContext.Comments.Where(c => c.Id == commentId).FirstOrDefaultAsync(); // FirstOrDefaultAsync + await
        }

        public async Task DeleteComment(CommentModel comment)
        {
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync(); // SaveChangesAsync + await
        }


        public async Task<List<CommentModel>> GetVideoComments(Guid videoId)
        {
            return await _dbContext.Comments.Where(c => c.VideoId == videoId).ToListAsync(); // ToListAsync + await

        }

        public async Task<List<CommentModel>> GetCommentResponses(Guid commentId)
        {
            return await _dbContext.Comments.Where(c => c.ParentCommentId == commentId).ToListAsync(); // ToListAsync + await
        }

        public async Task DeleteComments(IEnumerable<CommentModel> comments)
        {
            _dbContext.Comments.RemoveRange(comments);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(CommentModel comment)
        {
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CommentModel>> GetUserCommentsAsync(Guid userId)
        {
            return await _dbContext.Comments.Where(c => c.AuthorId == userId).ToListAsync();
        }
    }
}
