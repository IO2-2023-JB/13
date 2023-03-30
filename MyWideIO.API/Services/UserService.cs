using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MyVideIO.Models;
using MyWideIO.API.Data.IRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ViewerModel> _userManager;
        private readonly SignInManager<ViewerModel> _signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<ViewerModel> userManager, SignInManager<ViewerModel> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<bool> EditUserDataAsync(UserDto userDto, ClaimsPrincipal user)
        {
            var viewer = await _userManager.GetUserAsync(user);
            return true;
        }

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return string.Empty;
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (result.Succeeded)
                return await GenerateToken(user);
            return string.Empty;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto, ModelStateDictionary modelState)
        {
            ViewerModel viewer = new ViewerModel
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserName = registerDto.Nickname,
                Surname = registerDto.Surname
            };
            var result = await _userManager.CreateAsync(viewer, registerDto.Password);
            if (!result.Succeeded)
                foreach (var error in result.Errors)
                    modelState.AddModelError(error.Code, error.Description);
            await _userManager.AddToRoleAsync(viewer, Random.Shared.Next(3) switch // na szybko
            {
                0 => "viewer",
                1 => "creator",
                2 => "admin"
            });
            return result.Succeeded;
        }
        private async Task<string> GenerateToken(ViewerModel viewer)
        {
            var roles = await _userManager.GetRolesAsync(viewer);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, viewer.Name),
                new Claim(ClaimTypes.Email, viewer.Email),
                new Claim(ClaimTypes.NameIdentifier, viewer.UserName),
                new Claim(JwtRegisteredClaimNames.Sub,viewer.Id), // podobno id powinno byc w sub
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), // od kiedy wazny
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()), // do kiedy wazny, dla admina moze, na razie wazny 1 dzien
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TajnyKlucz128bit"));
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredencials = new SigningCredentials(key, algorithm);

            var header = new JwtHeader(signingCredencials);

            var payload = new JwtPayload(claims);

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
