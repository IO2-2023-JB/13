using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public interface IUserService
    {
        public Task RegisterUserAsync(RegisterDto registerDto);
        public Task<string> LoginUserAsync(LoginDto loginDto);
        public Task DeleteUserAsync(Guid id);
        public Task<UserDto> EditUserDataAsync(UpdateUserDto userDto, Guid id);
        public Task<UserDto> GetUserAsync(Guid id);
    }
}
