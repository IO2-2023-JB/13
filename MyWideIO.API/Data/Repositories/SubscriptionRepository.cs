using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using System.Data;
using System;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace MyWideIO.API.Data.Repositories
{
    public class SubscriptionRepository : Repository<ViewerSubscription>, ISubscriptionRepository
    {

        public SubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }



        public async Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subscriptions
                .Where(vs => vs.ViewerId == viewerId && vs.CreatorId == subId)
                .AnyAsync(cancellationToken);
        }

        public async Task<ViewerSubscription?> GetSubscriptionByIdAsync(Guid viewerId, Guid subId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subscriptions
                .Where(s => s.CreatorId == subId && s.ViewerId == viewerId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ViewerSubscription>> GetSubscriptionsToCreator(Guid creatorId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subscriptions
                .Where(s => s.CreatorId == creatorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ViewerSubscription>> GetViewersSubscriptionsAsync(Guid viewerId, bool includeVideos, CancellationToken cancellationToken = default)
        {
            IQueryable<ViewerSubscription> query = _dbContext.Subscriptions;
            query = includeVideos
                ? query.Include(s => s.Creator)
                .ThenInclude(c => c.OwnedVideos)
                .ThenInclude(v => v.Tags)
                : query.Include(s => s.Creator);
            return await query
                .Where(s => s.ViewerId == viewerId)
                .ToListAsync(cancellationToken);
        }
    }
}
