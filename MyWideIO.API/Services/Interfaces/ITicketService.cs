using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ITicketService
    {
        public Task<SubmitTicketResponseDto> CreateTicketAsync(SubmitTicketDto submitTicketDto, Guid userId);
        public Task<GetTicketDto> GetTicketAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken = default);
        public Task<GetTicketStatusDto> GetTicketStatusAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken = default);
        public Task<List<GetTicketDto>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<SubmitTicketResponseDto> AddResponseToTicketAsync(RespondToTicketDto respondToTicketDto, Guid ticketId);
    }
}
