using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ISubscriptionRepository : IRepository<ViewerSubscription>
    {
        public Task<List<ViewerSubscription>> GetViewersSubscriptionsAsync(Guid viewerId, bool includeVideos = false);
        public Task<List<ViewerSubscription>> GetSubscriptionsToCreator(Guid creatorId);

        public Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId);
        public Task<ViewerSubscription?> GetSubscriptionByIdAsync(Guid viewerId, Guid subId);
    }
}
