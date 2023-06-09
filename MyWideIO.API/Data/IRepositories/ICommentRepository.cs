using Azure;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ICommentRepository : IRepository<CommentModel>
    {
        public Task<List<CommentModel>> GetUserCommentsAsync(Guid userId);
        public Task<List<CommentModel>> GetVideoComments(Guid videoId);
        public Task<List<CommentModel>> GetCommentResponses(Guid commentId);
    }
}
