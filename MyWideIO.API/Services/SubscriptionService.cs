using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MyWideIO.API.Models.Enums;
using Microsoft.OpenApi.Extensions;
using MyWideIO.API.Exceptions;

namespace MyWideIO.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly UserManager<AppUserModel> _userManager; // ?
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(UserManager<AppUserModel> userManager, ISubscriptionRepository subscriptionRepository)
        {
            _userManager = userManager;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task SubscribeAsync(Guid viewerId, Guid subId) 
        {
            AppUserModel? creator = await _userManager.FindByIdAsync(subId.ToString());
            AppUserModel? viewer = await _userManager.FindByIdAsync(viewerId.ToString());

            // mozna zasubskrybowac samego siebie? // i guess

            if (await _subscriptionRepository.IsSubscribedAsync(viewerId, subId))
                throw new BadRequestException("already subscribed");

            UserTypeEnum userType;
           
            if(!Enum.TryParse((await _userManager.GetRolesAsync(creator)).First(), out userType) || userType is not UserTypeEnum.Creator) 
            {
                throw new NotCreatorException();
            }

            if (creator is not null && viewer is not null) 
            {
                var subscription = new ViewerSubscription
                {
                    ViewerId = viewerId,
                    CreatorId = subId,
                    Creator = creator
                };

                creator.SubscribersAmount++;
                await _userManager.UpdateAsync(creator);
                await _subscriptionRepository.AddSubscribtionAsync(subscription);
            }
            else
            {
                throw new UserNotFoundException();
            }

        }

        public async Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id) 
        {
            // repozytorium nie powinno mieć stycznosci z DTO, tylko z modelami
            // czyli zamiana Model na Dto powinna byc w serwisie
            return await _subscriptionRepository.GetSubscriptionsAsync(id);
        }

        public async Task UnsubscribeAsync(Guid vieverId, Guid subId) 
        {
            ViewerSubscription? sub = await _subscriptionRepository.GetSubscriptionByIdAsync(vieverId, subId);

            AppUserModel? creator = await _userManager.FindByIdAsync(subId.ToString());

            if (sub is not null && creator is not null)
            {
                // nie ma logiki w repozytorium
                await _subscriptionRepository.UnsubscribeAsync(sub);
                creator.SubscribersAmount--;
                await _userManager.UpdateAsync(creator);
            }
            else
            {
                throw new BadRequestException("");
            }

        }
    }
}
