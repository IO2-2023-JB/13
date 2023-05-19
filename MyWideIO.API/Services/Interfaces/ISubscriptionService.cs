using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        public Task Subscribe(Guid vieverId, Guid subId);
        public Task UnSubscribe(Guid vieverId, Guid subId);
        public Task<UserSubscriptionListDto> Subscriptions(Guid id);

    }
}
