using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ISubscriptionRepository
    {
        public Task AddSubscriptionAsync(ViewerSubscription subscription);
        public Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id);
        public Task<List<ViewerSubscription>> GetSubscriptionsToCreator(Guid creatorId);
        public Task UnsubscribeAsync(ViewerSubscription sub);
        public Task RemoveAsync(IEnumerable<ViewerSubscription> subscriptions);

        public Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId);
        public Task<ViewerSubscription?> GetSubscriptionByIdAsync(Guid viewerId, Guid subId);
    }
}
