using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IUserService
    {
        public Task RegisterUserAsync(RegisterDto registerDto);
        public Task<string> LoginUserAsync(LoginDto loginDto);
        public Task DeleteUserAsync(Guid id);
        public Task<UserDto> EditUserDataAsync(UpdateUserDto userDto, Guid id);
        public Task<UserDto> GetUserAsync(Guid id, Guid askerId);
    }
}
