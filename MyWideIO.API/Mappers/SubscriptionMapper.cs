using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Mappers
{
    public static class SubscriptionMapper
    {
        public static SubscriptionDto ToSubscriptionDto(this ViewerSubscription subscription)
        {
            return new SubscriptionDto
            {
                AvatarImage = subscription.Creator.ProfilePicture?.Url,
                Id = subscription.CreatorId,
                Nickname = subscription.Creator.UserName
            };
        }
    }
}
