using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Mappers;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<AppUserModel> _userManager;

        private readonly ITicketRepository _ticketRepository;

        public CommentService(ICommentRepository commentRepository, UserManager<AppUserModel> userManager, ITicketRepository ticketRepository)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _ticketRepository = ticketRepository;
        }
        public async Task AddResponseToComment(Guid commentId, string content, Guid userId)
        {
            CommentModel parent = await _commentRepository.GetAsync(commentId) ?? throw new CommentNotFoundException();
            if (parent.ParentCommentId is not null)
                throw new CommentException("Cant add a response to a response");
            await _commentRepository.AddAsync(new CommentModel()
            {
                Content = content,
                VideoId = parent.VideoId,
                AuthorId = userId,
                ParentCommentId = commentId,
                hasResponses = false
            });
            parent.hasResponses = true;
            await _commentRepository.UpdateAsync(parent);
        }

        public async Task AddNewComment(Guid videoId, string content, Guid userId)
        {
            await _commentRepository.AddAsync(new CommentModel()
            {
                Content = content,
                VideoId = videoId,
                AuthorId = userId,
                ParentCommentId = null,
                hasResponses = false
            });
        }

        public async Task DeleteComment(Guid commentId, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();
            CommentModel comment = await _commentRepository.GetAsync(commentId) ?? throw new CommentNotFoundException();
            if (comment.AuthorId != userId && !await _userManager.IsInRoleAsync(user, UserTypeEnum.Administrator.ToString()))
                throw new ForbiddenException();
            if (comment.hasResponses)
            {
                var responses = await _commentRepository.GetCommentResponses(commentId);
                foreach(var response in responses)
                {
                    var responseTickets = await _ticketRepository.GetTargetsTickets(response.Id);
                    await _ticketRepository.RemoveAsync(responseTickets, false);
                }
                await _commentRepository.RemoveAsync(responses, false);
            }

            var tickets = await _ticketRepository.GetTargetsTickets(commentId);
            await _ticketRepository.RemoveAsync(tickets);

            await _commentRepository.RemoveAsync(comment, false);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task<CommentListDto> GetVideoComments(Guid videoId)
        {
            List<CommentModel> comments = await _commentRepository.GetVideoComments(videoId);
            return new CommentListDto
            {
                Comments = comments.Select(c => c.ToCommentDto()).ToList()
            };
        }

        public async Task<CommentListDto> GetCommentResponses(Guid commentId)
        {
            List<CommentModel> comments = await _commentRepository.GetCommentResponses(commentId);
            return new CommentListDto
            {
                Comments = comments.Select(c => c.ToCommentDto()).ToList()
            };
        }
        public async Task<CommentDto> GetCommentById(Guid id)
        {
            var model = await _commentRepository.GetAsync(id) ?? throw new CommentNotFoundException();
            if (model.ParentCommentId != null)
                throw new Exception("Comment of given id is not a video comment (probably a response)"); // czemu propably
            AppUserModel user = await _userManager.FindByIdAsync(model.AuthorId.ToString());
            return new CommentDto()
            {
                Id = model.Id,
                AuthorId = model.AuthorId,
                Content = model.Content,
                AvatarImage = user.ProfilePicture?.Url,
                Nickname = user.UserName,
                HasResponses = model.hasResponses
            };
        }
        public async Task<CommentDto> GetCommentResponseById(Guid id)
        {
            var model = await _commentRepository.GetAsync(id) ?? throw new CommentNotFoundException();
            if (model.ParentCommentId == null)
                throw new Exception("Comment of given id is not a response (probably a video comment)");
            AppUserModel user = await _userManager.FindByIdAsync(model.AuthorId.ToString());
            return new CommentDto()
            {
                Id = model.Id,
                AuthorId = model.AuthorId,
                Content = model.Content,
                AvatarImage = user.ProfilePicture?.Url,
                Nickname = user.UserName,
                HasResponses = model.hasResponses
            };
        }
    }
}
