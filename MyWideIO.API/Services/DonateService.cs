using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using System.Reflection;

namespace MyWideIO.API.Services
{
    public class DonateService : IDonateService
    {
        private readonly UserManager<AppUserModel> _userManager;
        public DonateService(UserManager<AppUserModel> userManager)
        {
            _userManager = userManager;
        }
        public async Task SendDonation(Guid reciverId, decimal amount)
        {
            AppUserModel reciever = await _userManager.FindByIdAsync(reciverId.ToString()) ?? throw new UserNotFoundException();
            reciever.AccountBalance += amount;
            await _userManager.UpdateAsync(reciever);
        }

        public async Task Withdraw(Guid UserId, decimal amount)
        {
            AppUserModel user = await _userManager.FindByIdAsync(UserId.ToString()) ?? throw new UserNotFoundException();

            if (amount < 0)
                throw new NotEnoughMoneyException();

            if (user.AccountBalance < amount)
                throw new NotEnoughMoneyException();

            user.AccountBalance -= amount;
            await _userManager.UpdateAsync(user);
        }
    }
}
