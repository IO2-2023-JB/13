using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly ICommentRepository _commentRepository;

        public TicketService(ITicketRepository ticketRepository, IVideoRepository videoRepository, IPlaylistRepository playlistRepository, UserManager<AppUserModel> userManager, ICommentRepository commentRepository)
        {
            _ticketRepository = ticketRepository;
            _videoRepository = videoRepository;
            _playlistRepository = playlistRepository;
            _userManager = userManager;
            _commentRepository = commentRepository;
        }
        public async Task<SubmitTicketResponseDto> CreateTicketAsync(SubmitTicketDto submitTicketDto, Guid userId)
        {
            switch (submitTicketDto.TargetType)
            {
                case TicketTargetTypeEnum.User:
                    if (await _userManager.FindByIdAsync(submitTicketDto.TargetId.ToString()) is null)
                        throw new UserNotFoundException();
                    break;
                case TicketTargetTypeEnum.Video:
                    if (await _videoRepository.GetVideoAsync(submitTicketDto.TargetId) is null)
                        throw new VideoNotFoundException();
                    break;
                case TicketTargetTypeEnum.Playlist:
                    if (await _playlistRepository.GetPlaylistAsync(submitTicketDto.TargetId) is null)
                        throw new PlaylistNotFoundException();
                    break;
                case TicketTargetTypeEnum.CommentResponse: // moze jakies sprawdzenie czy to odpowiedz czy nie
                case TicketTargetTypeEnum.Comment:
                    if (await _commentRepository.GetComment(submitTicketDto.TargetId) is null)
                        throw new CommentNotFoundException();
                    break;
            }

            var ticket = new TicketModel
            {
                Reason = submitTicketDto.Reason,
                TargetId = submitTicketDto.TargetId,
                TargetType = submitTicketDto.TargetType,
                SubmitterId = userId,
                Status = TicketStatusEnum.Submitted
            };
            await _ticketRepository.AddTicketAsync(ticket);
            return new SubmitTicketResponseDto
            {
                Id = ticket.Id
            };
        }
        public async Task<GetTicketDto> GetTicketAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetTicketAsync(ticketId, cancellationToken) ?? throw new TicketNotFoundException();
            return TicketMapper.MapTicketModelToGetTicketDto(ticket);
        }
        public async Task<GetTicketStatusDto> GetTicketStatusAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetTicketAsync(ticketId, cancellationToken) ?? throw new TicketNotFoundException();
            return new GetTicketStatusDto
            {
                Status = ticket.Status
            };
        }
        public async Task<List<GetTicketDto>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken)
        {
            List<TicketModel> tickets;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == UserTypeEnum.Administrator.ToString())
                tickets = await _ticketRepository.GetSubbmitedTicketsAsync(cancellationToken);
            else
                tickets = await _ticketRepository.GetUserTicketsAsync(userId, cancellationToken);
            return tickets.Select(TicketMapper.MapTicketModelToGetTicketDto).ToList();
        }
        public async Task<SubmitTicketResponseDto> AddResponseToTicketAsync(RespondToTicketDto respondToTicketDto, Guid ticketId)
        {
            var ticket = await _ticketRepository.GetTicketAsync(ticketId) ?? throw new TicketNotFoundException();
            ticket.Response = respondToTicketDto.Response;
            ticket.Status = TicketStatusEnum.Resolved;
            await _ticketRepository.UpdateTicketAsync(ticket);
            return new SubmitTicketResponseDto
            {
                Id = ticket.Id
            };
        }
    }
}
