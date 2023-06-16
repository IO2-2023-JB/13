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

        public TicketService(
            ITicketRepository ticketRepository,
            IVideoRepository videoRepository,
            IPlaylistRepository playlistRepository,
            UserManager<AppUserModel> userManager,
            ICommentRepository commentRepository
            )
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
                    if (await _videoRepository.GetAsync(submitTicketDto.TargetId) is null)
                        throw new VideoNotFoundException();
                    break;
                case TicketTargetTypeEnum.Playlist:
                    if (await _playlistRepository.GetAsync(submitTicketDto.TargetId) is null)
                        throw new PlaylistNotFoundException();
                    break;
                case TicketTargetTypeEnum.CommentResponse: // moze jakies sprawdzenie czy to odpowiedz czy nie
                case TicketTargetTypeEnum.Comment:
                    if (await _commentRepository.GetAsync(submitTicketDto.TargetId) is null)
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
            await _ticketRepository.AddAsync(ticket);
            return new SubmitTicketResponseDto
            {
                Id = ticket.Id
            };
        }
        public async Task<GetTicketDto> GetTicketAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetAsync(ticketId, cancellationToken) ?? throw new TicketNotFoundException();
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();
            if (ticket.SubmitterId != userId && !await _userManager.IsInRoleAsync(user, UserTypeEnum.Administrator.ToString()))
                throw new ForbiddenException();
            return ticket.ToGetTicketDto();
        }
        public async Task<TicketStatusEnum> GetTicketStatusAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetAsync(ticketId, cancellationToken) ?? throw new TicketNotFoundException();
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();
            if (ticket.SubmitterId != userId && !await _userManager.IsInRoleAsync(user, UserTypeEnum.Administrator.ToString()))
                throw new ForbiddenException();
            return ticket.Status;
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
            return tickets.Select(t => t.ToGetTicketDto()).ToList();
        }
        public async Task<SubmitTicketResponseDto> AddResponseToTicketAsync(RespondToTicketDto respondToTicketDto, Guid ticketId)
        {
            var ticket = await _ticketRepository.GetAsync(ticketId) ?? throw new TicketNotFoundException();
            ticket.Response = respondToTicketDto.Response;
            ticket.Status = TicketStatusEnum.Resolved;
            await _ticketRepository.UpdateAsync(ticket);
            return new SubmitTicketResponseDto
            {
                Id = ticket.Id
            };
        }
    }
}
