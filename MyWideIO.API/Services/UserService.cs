using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using WideIO.API.Models;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ViewerModel> _userManager;
        private readonly SignInManager<ViewerModel> _signInManager;
        private readonly ITokenService _authenticationService;
        private readonly ITransactionService _transactionService;
        private readonly IImageService _imageService;

        public UserService(UserManager<ViewerModel> userManager, IImageService imageService, SignInManager<ViewerModel> signInManager, ITokenService authenticationService, ITransactionService transactionService)
        {
            _imageService = imageService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _transactionService = transactionService;
        }
        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
            IdentityResult result;
            await _transactionService.BeginTransactionAsync();
            try
            {
                ViewerModel viewer = new()
                {
                    Email = registerDto.Email,
                    Name = registerDto.Name,
                    UserName = registerDto.Nickname,
                    Surname = registerDto.Surname
                };
                result = await _userManager.CreateAsync(viewer, registerDto.Password);
                if (!result.Succeeded)
                {
                    throw (_userManager.ErrorDescriber.DuplicateEmail(registerDto.Email).Code == result.Errors.First().Code) ?
                         new DuplicateEmailException() : new UserException(result.Errors.First()?.Code);
                }

                result = await _userManager.AddToRoleAsync(viewer, registerDto.UserType switch
                {
                    UserTypeDto.Viewer => "viewer",
                    UserTypeDto.Creator => "creator",
                    UserTypeDto.Administrator => "admin",
                    _ => throw new UserException("Invalid user type")
                });
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }

                if (registerDto.AvatarImage.Length > 0)
                {
                    (viewer.ProfilePicture, viewer.ProfilePictureFileName) = await _imageService.UploadImageAsync(registerDto.AvatarImage, viewer.Id.ToString() + ".png");
                    if (viewer.ProfilePicture.Length == 0)
                    {
                        throw new UserException("Image upload error");
                    }

                    result = await _userManager.UpdateAsync(viewer);
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First()?.Code);
                    }
                }
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();

        }
        private readonly Dictionary<UserTypeDto, string> _roleDictionary = new()
        {
            { UserTypeDto.Viewer, "Viewer" },
            { UserTypeDto.Creator, "Creator" },
            { UserTypeDto.Administrator, "Admin" }
        };
        public async Task<UserDto> EditUserDataAsync(UpdateUserDto updateUserDto, Guid id)
        {
            ViewerModel viewer;
            await _transactionService.BeginTransactionAsync();
            try
            {
                IdentityResult result;
                // zmiania danych
                viewer = await _userManager.FindByIdAsync(id.ToString());
                viewer.Name = updateUserDto.Name;
                viewer.Surname = updateUserDto.Surname;
                viewer.UserName = updateUserDto.Nickname;

                // zmiana zdjecia
                (viewer.ProfilePicture, viewer.ProfilePictureFileName) = await _imageService.UploadImageAsync(updateUserDto.AvatarImage, viewer.Id.ToString() + ".png");
                if (viewer.ProfilePicture.Length == 0)
                {
                    throw new UserException("Image upload error");
                }

                result = await _userManager.UpdateAsync(viewer); // tutaj zapisanie zmian w bazie
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }

                // zmiana roli
                string role = (await _userManager.GetRolesAsync(viewer)).First() ?? throw new UserException("User has no role");
                string newRole = _roleDictionary[updateUserDto.UserType];
                if (newRole != role)
                {
                    result = await _userManager.RemoveFromRoleAsync(viewer, role);
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First()?.Code);
                    }

                    result = await _userManager.AddToRoleAsync(viewer, newRole);
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First()?.Code);
                    }
                }
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();
            return await ConvertUserModelToDto(viewer);
        }

        public async Task<UserDto> GetUserAsync(Guid id)
        {
            ViewerModel viewer = await _userManager.FindByIdAsync(id.ToString());
            return viewer == null
                ? throw new UserNotFoundException() // dziwne ze sie kompiluje
                : await ConvertUserModelToDto(viewer);
        }

        private async Task<UserDto> ConvertUserModelToDto(ViewerModel viewer)
        {
            return new UserDto
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
                },
                AvatarImage = viewer.ProfilePicture ?? ""
            };
        }

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            ViewerModel user = (await _userManager.FindByEmailAsync(loginDto.Email)) ??
                throw new UserNotFoundException();

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new IncorrectPasswordException();
            }

            // if (!(await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false)).Succeeded)
            //   throw new UserException("Couldn't sign in");

            return _authenticationService.GenerateToken(user, await _userManager.GetRolesAsync(user));
        }

        public async Task DeleteUserAsync(Guid id)
        {
            ViewerModel user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserException("User doesn't exist");// UserNotFoundException();

            IdentityResult result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new UserException(result.Errors.First()?.Code);
            }
        }
    }
}
