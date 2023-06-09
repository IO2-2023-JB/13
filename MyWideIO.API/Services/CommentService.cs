using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Mappers;
using MyWideIO.API.Exceptions;

namespace MyWideIO.API.Services
{
    public class CommentService : ICommentService
    {
        //ICommentRepository _commentRepository { get; set; } // ???
        //IUserService _userService { get; set; } // ???

        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<AppUserModel> _userManager;

        public CommentService(ICommentRepository commentRepository, UserManager<AppUserModel> userManager)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
        }


        //public CommentService(ICommentRepository commentRepository, IUserService userService)
        //{
        //    _commentRepository = commentRepository;
        //    _userService = userService;
        //}

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

        public async Task DeleteComment(Guid commentId)
        {
            CommentModel comment = await _commentRepository.GetAsync(commentId) ?? throw new CommentNotFoundException();
            if (comment.hasResponses)
            {
                var responses = await _commentRepository.GetCommentResponses(commentId);
                await _commentRepository.RemoveAsync(responses,false);
            }
            await _commentRepository.RemoveAsync(comment,false);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task<CommentListDto> GetVideoComments(Guid videoId)
        {
            CommentListDto comments = new CommentListDto()
            {
                Comments = new List<CommentDto>()
            };

            List<CommentModel> rawComments = await _commentRepository.GetVideoComments(videoId);
            foreach (var cmnt in rawComments.Where(c => c.ParentCommentId is null)) // mozna zrobic oddzielna funkcje - 'mapper', jak sie w kilku miejscach to samo robi
                                                                                    // tylko wtedy trzeba dodac pole Author w komentarzu i go includowac w repository
                                                                                    // i wtedy select zamiast foreach, bardziej czytelne
            {
                // UserDto user = await _userService.GetUserAsync(cmnt.AuthorId); // wtf
                AppUserModel user = await _userManager.FindByIdAsync(cmnt.AuthorId.ToString());
                comments.Comments.Add(new CommentDto()
                {
                    Id = cmnt.Id,
                    AuthorId = cmnt.AuthorId,
                    Content = cmnt.Content,
                    AvatarImage = user.ProfilePicture?.Url,
                    Nickname = user.UserName,
                    HasResponses = cmnt.hasResponses
                });
            }
            return comments;
        }

        public async Task<CommentListDto> GetCommentResponses(Guid commentId)
        {
            CommentListDto comments = new CommentListDto()
            {
                Comments = new List<CommentDto>()
            };

            List<CommentModel> rawComments = await _commentRepository.GetCommentResponses(commentId);
            foreach (var cmnt in rawComments)
            {
                AppUserModel user = await _userManager.FindByIdAsync(cmnt.AuthorId.ToString());
                comments.Comments.Add(new CommentDto()
                {
                    Id = cmnt.Id,
                    AuthorId = cmnt.AuthorId,
                    Content = cmnt.Content,
                    AvatarImage = user.ProfilePicture?.Url,
                    Nickname = user.UserName,
                    HasResponses = cmnt.hasResponses
                });
            }
            return comments;
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
