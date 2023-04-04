using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public interface IUserService
    {
        public Task<bool> RegisterUserAsync(RegisterDto registerDto,ModelStateDictionary modelState);
        public Task<bool> LoginUserAsync(LoginDto loginDto);
        public Task<bool> DeleteUserAsync(Guid id);
        public Task<UserDto> GetUser(Guid id);
        public Task<bool> PutUserData(UpdateUserDto dto, Guid id);
    }
}
