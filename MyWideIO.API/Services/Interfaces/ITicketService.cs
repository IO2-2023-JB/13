using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ITicketService
    {
        public Task<SubmitTicketResponseDto> CreateTicketAsync(SubmitTicketDto submitTicketDto, Guid userId);
        public Task<GetTicketDto> GetTicketAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken = default);
        public Task<TicketStatusEnum> GetTicketStatusAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken = default);
        public Task<List<GetTicketDto>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<SubmitTicketResponseDto> AddResponseToTicketAsync(RespondToTicketDto respondToTicketDto, Guid ticketId);
    }
}
