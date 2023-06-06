using Azure;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ICommentRepository
    {
        public Task AddComment(CommentModel model);
        public Task Update(CommentModel comment);
        public Task<CommentModel?> GetComment(Guid commentId);
        public Task<List<CommentModel>> GetUserCommentsAsync(Guid userId);
        public Task DeleteComment(CommentModel comment);
        public Task<List<CommentModel>> GetVideoComments(Guid videoId);
        public Task<List<CommentModel>> GetCommentResponses(Guid commentId);
        public Task DeleteComments(IEnumerable<CommentModel> comments);
    }
}
