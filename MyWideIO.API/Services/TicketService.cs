using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public async Task<SubmitTicketResponseDto> CreateTicketAsync(SubmitTicketDto submitTicketDto, Guid userId)
        {
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
        public async Task<GetTicketDto> GetTicketAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetTicketAsync(ticketId) ?? throw new TicketNotFoundException();
            return TicketMapper.MapTicketModelToGetTicketDto(ticket);
        }
        public async Task<GetTicketStatusDto> GetTicketStatusAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetTicketAsync(ticketId) ?? throw new TicketNotFoundException();
            return new GetTicketStatusDto
            {
                Status = ticket.Status
            };
        }
        public async Task<List<GetTicketDto>> GetUserTicketsAsync(Guid userId)
        {
            var tickets = await _ticketRepository.GetUserTicketsAsync(userId);
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
