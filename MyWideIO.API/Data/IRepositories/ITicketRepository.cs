using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ITicketRepository
    {
        public Task AddAsync(TicketModel ticket);
        public Task UpdateAsync(TicketModel ticket);
        public Task RemoveAsync(TicketModel ticket);
        public Task RemoveAsync(IEnumerable<TicketModel> tickets);
        public Task<TicketModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<List<TicketModel>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<List<TicketModel>> GetSubbmitedTicketsAsync(CancellationToken cancellationToken  = default);
        public Task<List<TicketModel>> GetTargetsTickets(Guid targetId, CancellationToken cancellationToken = default);
        public Task<bool> TargetHasTickets(Guid targetId, CancellationToken cancellationToken = default);
        
    }
}
