using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(AppUserModel user, IEnumerable<string> roles);
    }
}
