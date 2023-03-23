using Microsoft.AspNetCore.Identity;
using MyVideIO.Models;
using MyWideIO.API.Data.IRepositories;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ViewerModel> _userManager;
        private readonly SignInManager<ViewerModel> _signInManager;
        public UserService(UserManager<ViewerModel> userManager, SignInManager<ViewerModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            ViewerModel viewer = new ViewerModel
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                Nick = registerDto.Nickname,
                Surname = registerDto.Surname
            };
            var result = await _userManager.CreateAsync(viewer, registerDto.Password);

            return result.Succeeded;
        }
    }
}
