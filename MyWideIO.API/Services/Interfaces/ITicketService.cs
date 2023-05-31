using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ITicketService
    {
        public Task<SubmitTicketResponseDto> CreateTicketAsync(SubmitTicketDto submitTicketDto, Guid userId);
        public Task<GetTicketDto> GetTicketAsync(Guid ticketId);
        public Task<GetTicketStatusDto> GetTicketStatusAsync(Guid ticketId);
        public Task<List<GetTicketDto>> GetUserTicketsAsync(Guid userId);
        public Task<SubmitTicketResponseDto> AddResponseToTicketAsync(RespondToTicketDto respondToTicketDto, Guid ticketId);
    }
}
