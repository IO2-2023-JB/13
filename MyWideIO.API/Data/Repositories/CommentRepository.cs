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
            if(response)
            {
                _dbContext.Comments.Where(c => c.Id == model.ParentCommentId).FirstOrDefault().hasResponses = true;
            }
            _dbContext.SaveChanges();
        }


        public async Task<CommentModel?> GetComment(Guid commentId)
        {
            return _dbContext.Comments.Where(c => c.Id == commentId).FirstOrDefault();
        }

        public async Task DeleteComment(CommentModel comment)
        {
            _dbContext.Remove(comment);
            _dbContext.SaveChanges();
        }


        public async Task<List<CommentModel>> GetVideoComments(Guid videoId)
        {
            return _dbContext.Comments.Where(c => c.VideoId == videoId).ToList();

        }

        public async Task<List<CommentModel>> GetCommentResponses(Guid commentId)
        {
            return _dbContext.Comments.Where(c => c.ParentCommentId == commentId).ToList();
        }
    }
}
