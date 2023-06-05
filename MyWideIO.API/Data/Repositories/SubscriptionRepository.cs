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
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubscriptionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // public async Task AddSubscriptionAsync(ViewerSubscription subscription)
        public async Task AddSubscriptionAsync(ViewerSubscription subscription) // SubscribeAsync, albo AddSubscriptionAsync
        {
            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();
        }
        // ewentualnie mozna zwracac IQuerable
        // public async Task<IEnumerable/IList/List/ICollection/Collection> GetSubscruptionsAsync(Guid viewerId)
        public async Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id) // GetSubscriptionsAsync
        {
            // return await _dbContext.Subscriptions.Where(s => s.ViewerId == id).Include(s => s.Creator).ToListAsync(); 

            AppUserModel? user = _dbContext.Users.Find(id);

            if (user != null)
            {
                // nie ma logiki w repozytorium
                List<SubscriptionDto> subscriptions = new List<SubscriptionDto>();
                foreach (var sub in _dbContext.Subscriptions.Where(s => s.ViewerId == id).ToList()) // najpierw ToList, a potem enumerowanie?
                {
                    var creator = _dbContext.Users.Find(sub.CreatorId);
                    if (creator == null) // konto twórcy zostało usunięte
                                         // no to przy usuwaniu konta tworcy powinny byc usuwane subskrypcje
                                         // albo przy zmianie typu konta na widza
                    {
                        // nie ma logiki w repozytorium
                        _dbContext.Subscriptions.Remove(sub);
                        continue;
                    }

                    subscriptions.Add(new SubscriptionDto() { Id = sub.CreatorId, Nickname = creator.UserName, AvatarImage = creator.ProfilePicture?.Url });
                    // nie ma dto w repozytorium
                }
                return new UserSubscriptionListDto() { Subscriptions = subscriptions }; // nie ma dto w repozytorium

            }
            else
            {
                throw new DataException("no User with specified Id"); // co to za wyjatek
            }

        }
        // public async Task RemoveSubscriptionAsync(ViewerSubscription subscription)
        public async Task UnsubscribeAsync(ViewerSubscription sub) // UnSubscribeAsync, albo RemoveSubscriptionAsync
        {
            _dbContext.Subscriptions.Remove(sub);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> IsSubscribedAsync(Guid viewerId, Guid subId)
        {
            return await _dbContext.Subscriptions.AnyAsync(vs => vs.ViewerId == viewerId && vs.CreatorId == subId);
        }

        public async Task<ViewerSubscription?> GetSubscriptionByIdAsync(Guid viewerId, Guid subId)
        {
            return await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.CreatorId == subId && s.ViewerId == viewerId);
        }
    }
}
