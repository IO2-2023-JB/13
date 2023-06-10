using Azure;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ICommentRepository : IRepository<CommentModel>
    {
        public Task<CommentModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<List<CommentModel>> GetUserCommentsAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<List<CommentModel>> GetVideoComments(Guid videoId, CancellationToken cancellationToken = default);
        public Task<List<CommentModel>> GetCommentResponses(Guid commentId, CancellationToken cancellationToken = default);
    }
}
