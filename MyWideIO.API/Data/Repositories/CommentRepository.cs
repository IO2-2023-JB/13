using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class CommentRepository: ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddComment(CommentModel model, bool response = false)
        {
             _dbContext.Comments.Add(model);
            if (response) // if(model.ParentCommentId is not null)
            {
                _dbContext.Comments.Where(c => c.Id == model.ParentCommentId).FirstOrDefault().hasResponses = true; // First zamiast FirstOrDefault
            }
            _dbContext.SaveChanges(); // SaveChangesAsync + await
        }


        public async Task<CommentModel?> GetComment(Guid commentId)
        {
            return _dbContext.Comments.Where(c => c.Id == commentId).FirstOrDefault(); // FirstOrDefaultAsync + await
        }

        public async Task DeleteComment(CommentModel comment)
        {
            _dbContext.Remove(comment);
            _dbContext.SaveChanges(); // SaveChangesAsync + await
        }


        public async Task<List<CommentModel>> GetVideoComments(Guid videoId)
        {
            return _dbContext.Comments.Where(c => c.VideoId == videoId).ToList(); // ToListAsync + await

        }

        public async Task<List<CommentModel>> GetCommentResponses(Guid commentId)
        {
            return _dbContext.Comments.Where(c => c.ParentCommentId == commentId).ToList(); // ToListAsync + await
        }
    }
}
