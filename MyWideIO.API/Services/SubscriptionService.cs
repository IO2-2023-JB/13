using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using System.Data;
using System;
using System.Security.Claims;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(UserManager<AppUserModel> userManager, ISubscriptionRepository subscriptionRepository) 
        { 
            _userManager = userManager;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task Subscribe(Guid vieverId, Guid subId)
        {
            await _subscriptionRepository.Subscribe(vieverId, subId);
        }

        public async Task<UserSubscriptionListDto> Subscriptions(Guid id)
        {
            return await _subscriptionRepository.Subscriptions(id);
        }

        public async Task UnSubscribe(Guid vieverId, Guid subId)
        {
            await _subscriptionRepository.UnSubscribe(vieverId, subId);
        }
    }
}
