using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ISubscriptionRepository
    {
        public Task AddSubscribtionAsync(ViewerSubscription subscribtion);
        public Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id);
        public Task UnsubscribeAsync(ViewerSubscription sub);
        public Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId);
        public Task<ViewerSubscription> GetSubscriptionByIdAsync(Guid viewerId, Guid subId);
    }
}
