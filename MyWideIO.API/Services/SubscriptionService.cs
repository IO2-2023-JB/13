using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.Dto_Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Mappers;
using Microsoft.OpenApi.Extensions;
using MyWideIO.API.Exceptions;

namespace MyWideIO.API.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(
            UserManager<AppUserModel> userManager,
            ISubscriptionRepository subscriptionRepository
            )
        {
            _userManager = userManager;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task SubscribeAsync(Guid viewerId, Guid subId)
        {
            if (viewerId == subId)
                throw new CustomException("Can't subscribe to yourself");
            AppUserModel creator = await _userManager.FindByIdAsync(subId.ToString()) ?? throw new UserNotFoundException();
            var viewer = await _userManager.FindByIdAsync(viewerId.ToString()) ?? throw new UserNotFoundException();


            if (await _subscriptionRepository.IsSubscribedAsync(viewerId, subId))
                throw new BadRequestException("already subscribed");

            if (!await _userManager.IsInRoleAsync(creator, UserTypeEnum.Creator.ToString()))
                throw new NotCreatorException();
            var subscription = new ViewerSubscription
            {
                ViewerId = viewerId,
                CreatorId = subId,
                Creator = creator, //
                Viewer = viewer // 
            };

            creator.SubscribersAmount++;
            await _userManager.UpdateAsync(creator);
            await _subscriptionRepository.AddAsync(subscription);
        }

        public async Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid id)
        {
            var subsriptions = await _subscriptionRepository.GetViewersSubscriptionsAsync(id);

            return new UserSubscriptionListDto
            {
                Subscriptions = subsriptions.Select(s => s.ToSubscriptionDto()).ToList()
            };
        }

        public async Task UnsubscribeAsync(Guid vieverId, Guid subId)
        {
            ViewerSubscription sub = await _subscriptionRepository.GetSubscriptionByIdAsync(vieverId, subId) ?? throw new NotFoundException("S");

            AppUserModel creator = await _userManager.FindByIdAsync(subId.ToString());

            await _subscriptionRepository.RemoveAsync(sub);
            creator.SubscribersAmount--;
            await _userManager.UpdateAsync(creator);
        }
    }
}
