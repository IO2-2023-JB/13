using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MyWideIO.API.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
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
        private readonly RoleManager<UserRole> _roleManager;

        public UserService(UserManager<ViewerModel> userManager, SignInManager<ViewerModel> signInManager, RoleManager<UserRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task EditUserDataAsync(UserDto userDto)
        {
            IdentityResult result;
            var viewer = await _userManager.FindByIdAsync(userDto.Id.ToString()); // xd guid -> string -> guid

            var emailToken = await _userManager.GenerateChangeEmailTokenAsync(viewer, userDto.Email);
            result = await _userManager.ChangeEmailAsync(viewer, userDto.Email, emailToken);
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);

            viewer.Name = userDto.Name;
            viewer.Surname = userDto.Surname;
            viewer.UserName = userDto.Nickname;

            result = await _userManager.UpdateAsync(viewer);
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);

            var role = (await _userManager.GetRolesAsync(viewer)).First();
            string newRole = "";
            switch (userDto.UserType)
            {
                case UserTypeDto.Viewer:
                    if (role != "Viewer")
                        newRole = "Viewer";
                    break;
                case UserTypeDto.Creator:
                    if (role != "Creator")
                        newRole = "Creator";
                    break;
                case UserTypeDto.Administrator:
                    if (role != "Admin")
                        newRole = "Admin";
                    break;
            }
            if (newRole.Length > 0)
            {
                result = await _userManager.RemoveFromRoleAsync(viewer, role);
                if (!result.Succeeded)
                    throw new UserException(result.Errors.First()?.Code);
                result = await _userManager.AddToRoleAsync(viewer, newRole);
                if (!result.Succeeded)
                    throw new UserException(result.Errors.First()?.Code);
            }
        }

        public async Task<UserDto> GetUserAsync(Guid id)
        {
            var viewer = await _userManager.FindByIdAsync(id.ToString());
            return viewer == null
                ? throw new UserNotFoundException() // dziwne ze sie kompiluje
                : new UserDto
                {
                    Id = viewer.Id,
                    Name = viewer.Name,
                    Surname = viewer.Surname,
                    Email = viewer.Email,
                    Nickname = viewer.UserName,
                    UserType = (await _userManager.GetRolesAsync(viewer)).First() switch
                    {
                        "Viewer" => UserTypeDto.Viewer,
                        "Creator" => UserTypeDto.Creator,
                        "Admin" => UserTypeDto.Administrator
                    }
                };
        }


        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var user = (await _userManager.FindByEmailAsync(loginDto.Email)) ??
                throw new UserNotFoundException();
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new IncorrectPasswordException();
            if (!(await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false)).Succeeded)
                throw new UserException("Couldn't sign in");
            return await GenerateToken(user);
        }

        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
            IdentityResult result;
            ViewerModel viewer = new ViewerModel
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserName = registerDto.Nickname,
                Surname = registerDto.Surname,

            };
            result = await _userManager.CreateAsync(viewer, registerDto.Password);
            if (!result.Succeeded)
                throw (_userManager.ErrorDescriber.DuplicateEmail(registerDto.Email).Code == result.Errors.First().Code) ?
                     new DuplicateEmailException() : new UserException(result.Errors.First()?.Code);

            result = await _userManager.AddToRoleAsync(viewer, registerDto.UserType switch // czemu warning jest
            {
                UserTypeDto.Viewer => "viewer",
                UserTypeDto.Creator => "creator",
                UserTypeDto.Administrator => "admin"
            });
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);
        }
        private async Task<string> GenerateToken(ViewerModel viewer)
        {
            var roles = await _userManager.GetRolesAsync(viewer);
            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, viewer.Name),
                //new Claim(ClaimTypes.Email, viewer.Email),
                //new Claim(ClaimTypes.NameIdentifier, viewer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,viewer.Id.ToString()), // podobno id powinno byc w sub
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), // od kiedy wazny
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()), // do kiedy wazny
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
