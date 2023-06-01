using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ISubscriptionRepository
    {
        public Task Subscribe(Guid vieverId, Guid subId);
        public Task<UserSubscriptionListDto> Subscriptions(Guid id);
        public Task UnSubscribe(Guid vieverId, Guid subId);

    }
}
