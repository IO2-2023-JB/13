using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TicketRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TicketModel ticket)
        {
            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TicketModel>> GetSubbmitedTicketsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Where(t => t.Status == Models.Enums.TicketStatusEnum.Submitted)
                .ToListAsync(cancellationToken);
        }

        public async Task<TicketModel?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets
                .FindAsync(new object?[] { id },  cancellationToken); // xd
        }

        public async Task<List<TicketModel>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets
                .Where(t=>t.SubmitterId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task RemoveAsync(TicketModel ticket)
        {
            _dbContext.Remove(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TicketModel ticket)
        {
            _dbContext.Update(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(IEnumerable<TicketModel> tickets)
        {
            _dbContext.Tickets.RemoveRange(tickets);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TicketModel>> GetTargetsTickets(Guid targetId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Where(t => t.TargetId == targetId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> TargetHasTickets(Guid targetId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Where(t => t.TargetId == targetId)
                .AnyAsync(cancellationToken);
        }
    }
}
