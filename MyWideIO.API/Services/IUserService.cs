using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public interface IUserService
    {
        public Task<bool> RegisterUserAsync(RegisterDto registerDto,ModelStateDictionary modelState);
        public Task<string> LoginUserAsync(LoginDto loginDto);
        public Task<(bool Suceeded, IEnumerable<IdentityError> Errors)> EditUserDataAsync(UserDto userDto);
        public Task<UserDto?> GetUserAsync(Guid id);
    }
}
