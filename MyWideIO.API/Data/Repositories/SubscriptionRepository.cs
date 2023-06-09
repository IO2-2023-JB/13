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



        public async Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId)
        {
            return await _dbContext.Subscriptions.AnyAsync(vs => vs.ViewerId == viewerId && vs.CreatorId == subId);
        }

        public async Task<ViewerSubscription?> GetSubscriptionByIdAsync(Guid viewerId, Guid subId)
        {
            return await _dbContext.Subscriptions
                .Where(s => s.CreatorId == subId && s.ViewerId == viewerId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ViewerSubscription>> GetSubscriptionsToCreator(Guid creatorId)
        {
            return await _dbContext.Subscriptions
                .Where(s => s.CreatorId == creatorId)
                .ToListAsync();
        }

        public async Task<List<ViewerSubscription>> GetViewersSubscriptionsAsync(Guid viewerId, bool includeVideos)
        {
            IQueryable<ViewerSubscription> query = _dbContext.Subscriptions;
            query = includeVideos
                ? query.Include(s => s.Creator).ThenInclude(c => c.OwnedVideos) // include i theninclude zwracaja rozne typy
                : query.Include(s => s.Creator);
            return await query.Where(s => s.ViewerId == viewerId).ToListAsync();
        }
    }
}
