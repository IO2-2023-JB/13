using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        public Task SubscribeAsync(Guid vieverId, Guid subId);
        public Task UnsubscribeAsync(Guid vieverId, Guid subId);
        public Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id);

    }
}
