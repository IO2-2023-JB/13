using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ICommentService
    {
        public Task AddNewComment(Guid videoId, string content, Guid userId);
        public Task AddResoponseToComment(Guid commentId, string content, Guid userId);
        public Task DeleteComment(Guid commentId);
        public Task<CommentListDto> GetVideoComments(Guid videoId);
        public Task<CommentListDto> GetCommentResponses(Guid commentId);
    }
}
