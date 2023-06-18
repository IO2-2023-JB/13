using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ICommentService
    {
        public Task AddNewComment(Guid videoId, string content, Guid userId);
        public Task AddResponseToComment(Guid commentId, string content, Guid userId);
        public Task DeleteComment(Guid commentId, Guid userId);
        public Task<CommentListDto> GetVideoComments(Guid videoId);
        public Task<CommentListDto> GetCommentResponses(Guid commentId);
        public Task<CommentDto> GetCommentById(Guid id);
        public Task<CommentDto> GetCommentResponseById(Guid id);
    }
}
