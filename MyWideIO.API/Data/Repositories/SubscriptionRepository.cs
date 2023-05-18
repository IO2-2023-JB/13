using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using System.Data;
using System;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task Subscribe(Guid viewerId, Guid subId) // SubscribeAsync, albo AddSubscriptionAsync
        {
            AppUserModel? creator = _dbContext.Users.Find(subId); // to powinno byc w serwisie, z uzyciem usermanager
            AppUserModel? viewer = _dbContext.Users.Find(viewerId); // tak samo
            // mozna zasubskrybowac samego siebie?
            // gdzie sprawdzenie czy creator ma rolę creatora, chyba ze mozna zasubskrybowac viewera
            if (creator != null && viewer != null) // raczej creator is null / is not null, jest to to samo, ale == i != mozna zoverloadowac
                                                   // albo ??
            {
                // nie ma logiki w repozytorium
                var subscription = new ViewerSubscription
                {
                    ViewerId = viewerId,
                    CreatorId = subId,
                    Creator = creator,
                    // Viewer = viewer ?
                };
                _dbContext.Subscriptions.Add(subscription);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new DataException("no User with specified Id"); // co to za wyjatek
            }
        }
        // ewentualnie mozna zwracac IQuerable, ale w tym projektcie nigdzie tak nie ma
        // public async Task<IEnumerable/IList/List/ICollection/Collection> GetSubscruptionsAsync(Guid viewerId)
        public async Task<UserSubscriptionListDto> Subscriptions(Guid id) // GetSubscriptionsAsync
        {
            AppUserModel? user = _dbContext.Users.Find(id);

            if (user != null)
            {
                // nie ma logiki w repozytorium
                List<SubscribtionDto> subscriptions = new List<SubscribtionDto>();
                foreach (var sub in _dbContext.Subscriptions.Where(s => s.ViewerId == id).ToList())
                {
                    var creator = _dbContext.Users.Find(sub.CreatorId);
                    if (creator == null) //konto twórcy zostało usunięte
                                         // no to przy usuwaniu konta tworcy powinny byc usuwane subskrypcje
                                         // albo przy zmianie typu konta na widza
                    {
                        // nie ma logiki w repozytorium
                        _dbContext.Subscriptions.Remove(sub);
                        continue;
                    }

                    subscriptions.Add(new SubscribtionDto() { Id = sub.CreatorId, Nickname = creator.UserName, AvatarImage = creator.ProfilePicture?.Url });
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
        public async Task UnSubscribe(Guid viewerId, Guid subId) // UnSubscribeAsync, albo RemoveSubscriptionAsync
        {
            ViewerSubscription? sub = _dbContext.Subscriptions.Where(s => s.CreatorId == subId && s.ViewerId == viewerId).First();
            // do tego powinna byc oddzielna metoda np GetSubscriptionAsync(Guid viewerId, Guid subId) zwracajaca Task<ViewerSubscription?>
            // i logika w serwisie


            //// AppUserModel? creator = _dbContext.Users.Find(subId);
            // AppUserModel? viewer = _dbContext.Users.Find(viewerId);
            //if (creator != null && viewer != null)
            //{
            //    var subscription = viewer.Subscriptions.FirstOrDefault(s => s.ViewerId == viewerId && s.CreatorId == subId);
            //    if(subscription == null) 
            //    {
            //        throw new Exception("no subscription"); 
            //    }
            //    creator.Subscribers.Remove(subscription);
            //    viewer.Subscriptions.Remove(subscription);
            //    _dbContext.Update(viewer);
            //    _dbContext.Update(creator);
            //    _dbContext.SaveChanges();
            //}
            if (sub != null)
            {
                // nie ma logiki w repozytorium
                _dbContext.Subscriptions.Remove(sub);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new DataException("no User with specified Id"); // co to za wyjatek
            }
        }
    }
}
