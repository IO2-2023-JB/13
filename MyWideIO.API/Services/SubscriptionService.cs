using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Models.Dto_Models;

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

        public async Task Subscribe(Guid vieverId, Guid subId) // metody asynchroniczne powiwinny miec Async na koncu nazwy: SubscribeAsync
        {
            // 'logika biznesowa' powinna byc w serwisie, a nie w kontrolerze, ani w repozytorium
            // oddzielne repozytorium powinno miec stycznosc z jedna tabela w bazie danych
            // po co robic service jak sie tylko przekazuje dalej?
            await _subscriptionRepository.Subscribe(vieverId, subId);
        }

        public async Task<UserSubscriptionListDto> Subscriptions(Guid id) // nieczytelna nazwa, lepsza bylaby GetSubscriptionsAsync
        {
            // repozytorium nie powinno mieć stycznosci z DTO, tylko z modelami
            // czyli zamiana Model na Dto powinna byc w serwisie
            return await _subscriptionRepository.Subscriptions(id);
        }

        public async Task UnSubscribe(Guid vieverId, Guid subId) // UnSubscribeAsync
        {
            // s = getSubscription(vieverId, subId)
            // removesubscription(s)
            await _subscriptionRepository.UnSubscribe(vieverId, subId);
        }
    }
}
