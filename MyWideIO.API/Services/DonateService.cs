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
        private readonly IDonateRepository _donateRepository;
        public DonateService(UserManager<AppUserModel> userManager, IDonateRepository donateRepository)
        {
            _userManager = userManager;
            _donateRepository = donateRepository;
        }
        public async Task SendDonation(Guid reciverId, Guid senderId, decimal amount)
        {
            AppUserModel reciever, sender;
            if ((reciever = await _userManager.FindByIdAsync(reciverId.ToString())) is null)
                throw new UserNotFoundException();
            sender = await _userManager.FindByIdAsync(senderId.ToString());

            if(sender.AccountBalance - amount < 0)
                throw new NotEnoughMoneyException();
            sender.AccountBalance -= amount;
            reciever.AccountBalance += amount;
            await _userManager.UpdateAsync(sender);
            await _userManager.UpdateAsync(reciever);
        }

        public async Task Withdraw(Guid UserId, decimal amount)
        {
            AppUserModel user = await _userManager.FindByIdAsync(UserId.ToString());

            //if(amount < 0)
                //throw new NotEnoughMoneyException(); //jakoś pieniążki trzeba zarabiać

            if (user.AccountBalance - amount < 0)
                throw new NotEnoughMoneyException();

            user.AccountBalance -= amount;
            await _userManager.UpdateAsync(user);
        }
    }
}
