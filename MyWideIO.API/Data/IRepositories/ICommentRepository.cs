using Azure;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ICommentRepository
    {
        public Task AddComment(CommentModel model, bool response = false);
        public Task<CommentModel> GetComment(Guid commentId);
        public Task DeleteComment(CommentModel comment);
        public Task<List<CommentModel>> GetVideoComments(Guid videoId);
        public Task<List<CommentModel>> GetCommentResponses(Guid commentId);
    }
}
