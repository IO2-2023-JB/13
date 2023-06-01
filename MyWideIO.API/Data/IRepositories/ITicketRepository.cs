using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ITicketRepository
    {
        public Task AddTicketAsync(TicketModel ticket);
        public Task UpdateTicketAsync(TicketModel ticket);
        public Task RemoveTicketAsync(TicketModel ticket);
        public Task<TicketModel?> GetTicketAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<List<TicketModel>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<List<TicketModel>> GetSubbmitedTicketsAsync(CancellationToken cancellationToken  = default);
        
    }
}
