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

        public async Task AddTicketAsync(TicketModel ticket)
        {
            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TicketModel?> GetTicketAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets.FindAsync(new object?[] { id },  cancellationToken); // xd
        }

        public async Task<List<TicketModel>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets.Where(t=>t.SubmitterId == userId).ToListAsync(cancellationToken);
        }

        public async Task RemoveTicketAsync(TicketModel ticket)
        {
            _dbContext.Remove(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(TicketModel ticket)
        {
            _dbContext.Update(ticket);
            await _dbContext.SaveChangesAsync();
        }
    }
}
