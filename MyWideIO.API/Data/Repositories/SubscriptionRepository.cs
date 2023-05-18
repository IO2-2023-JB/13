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

        public async Task Subscribe(Guid viewerId, Guid subId)
        {
            AppUserModel? creator = _dbContext.Users.Find(subId);
            AppUserModel? viewer = _dbContext.Users.Find(viewerId);
            if (creator != null && viewer != null)
            {
                var subscription = new ViewerSubscription
                {
                    ViewerId = viewerId, 
                    CreatorId = subId,
                    Creator = creator
                };
                _dbContext.Subscriptions.Add(subscription);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new DataException("no User with specified Id");
            }
        }

        public async Task<UserSubscriptionListDto> Subscriptions(Guid id)
        {
            AppUserModel? user = _dbContext.Users.Find(id);
            
            if (user != null)
            {
                List<SubscribtionDto> subscriptions = new List<SubscribtionDto>();
                foreach (var sub in _dbContext.Subscriptions.Where(s => s.ViewerId == id).ToList()) 
                {
                    var creator = _dbContext.Users.Find(sub.CreatorId);
                    if (creator == null) //konto twórcy zostało usunięte
                    { 
                        _dbContext.Subscriptions.Remove(sub); 
                        continue; 
                    }

                    subscriptions.Add(new SubscribtionDto() { Id = sub.CreatorId, Nickname = creator.UserName, AvatarImage = creator.ProfilePicture?.Url });
                }
                return new UserSubscriptionListDto() { Subscriptions = subscriptions };
                
            }
            else
            {
                throw new DataException("no User with specified Id");
            }

        }

        public async Task UnSubscribe(Guid viewerId, Guid subId)
        {
            ViewerSubscription? sub = _dbContext.Subscriptions.Where(s => s.CreatorId == subId && s.ViewerId == viewerId).First();
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
                _dbContext.Subscriptions.Remove(sub);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new DataException("no User with specified Id");
            }
        }
    }
}
