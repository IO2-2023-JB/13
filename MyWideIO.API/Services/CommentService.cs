﻿using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.AspNetCore.Identity;

namespace MyWideIO.API.Services
{
    public class CommentService : ICommentService
    {
        ICommentRepository _commentRepository { get; set; }
        IUserService _userService { get; set; }

        public CommentService(ICommentRepository commentRepository, IUserService userService)
        {
            _commentRepository = commentRepository;
            _userService = userService;
        }

        public async Task AddResoponseToComment(Guid commentId, string content, Guid userId)
        {
            await _commentRepository.AddComment(new CommentModel()
            {
                Id = new Guid(),
                Content = content,
                VideoId = null,
                AuthorId = userId,
                ParentCommentId = commentId,
                hasResponses = false
            }, true);
        }

        public async Task AddNewComment(Guid videoId, string content, Guid userId)
        {
            await _commentRepository.AddComment(new CommentModel()
            {
                Id = new Guid(),
                Content = content,
                VideoId = videoId,
                AuthorId = userId,
                ParentCommentId = null,
                hasResponses = false
            }, false);
        }

        public async Task DeleteComment(Guid commentId)
        {
            await _commentRepository.DeleteComment(
                await _commentRepository.GetComment(commentId)
                );
        }

        public async Task<CommentListDto> GetVideoComments(Guid videoId)
        {
            CommentListDto comments = new CommentListDto()
            {
                Comments = new List<CommentDto>()
            };

            var rawComments = await _commentRepository.GetVideoComments(videoId);
            foreach (var cmnt in rawComments)
            {
                UserDto user = await _userService.GetUserAsync(cmnt.AuthorId);
                comments.Comments.Add(new CommentDto()
                {
                    Id = cmnt.Id,
                    AuthorId = cmnt.AuthorId,
                    Content = cmnt.Content,
                    AvatarImage = user.AvatarImage,
                    Nickname = user.Nickname,
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

            var rawComments = await _commentRepository.GetCommentResponses(commentId);
            foreach (var cmnt in rawComments)
            {
                UserDto user = await _userService.GetUserAsync(cmnt.AuthorId);
                comments.Comments.Add(
                    new CommentDto()
                    {
                        Id = cmnt.Id,
                        AuthorId = cmnt.AuthorId,
                        Content = cmnt.Content,
                        AvatarImage = user.AvatarImage,
                        Nickname = user.Nickname,
                        HasResponses = cmnt.hasResponses
                    });
            }
            return comments;
        }
    }
}