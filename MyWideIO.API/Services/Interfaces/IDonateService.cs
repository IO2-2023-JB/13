using MyWideIO.API.Models.Dto_Models;
using System.Runtime.CompilerServices;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IDonateService
    {
        public Task SendDonation(Guid reciverId, decimal amount);
        public Task Withdraw(Guid UserId, decimal amount);
    }
}
