using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class TicketRepository : Repository<TicketModel>, ITicketRepository
    {

        public TicketRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<List<TicketModel>> GetSubbmitedTicketsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Where(t => t.Status == Models.Enums.TicketStatusEnum.Submitted)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TicketModel>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets
                .Where(t => t.SubmitterId == userId)
                .ToListAsync(cancellationToken);
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

        public async Task<TicketModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
