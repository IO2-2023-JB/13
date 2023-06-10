using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this CommentModel commentModel)
        {
            return new CommentDto
            {
                AuthorId = commentModel.AuthorId,
                AvatarImage = commentModel.Author.ProfilePicture?.Url,
                Content = commentModel.Content,
                HasResponses = commentModel.hasResponses,
                Id = commentModel.Id,
                Nickname = commentModel.Author.UserName
            };
        }
    }
}
