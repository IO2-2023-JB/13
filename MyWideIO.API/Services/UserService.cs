using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update.Internal;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly ITokenService _authenticationService;
        private readonly ITransactionService _transactionService;
        private readonly IImageStorageService _imageService;
        private readonly IVideoService _videoService;

        public UserService(UserManager<AppUserModel> userManager, IImageStorageService imageService, SignInManager<AppUserModel> signInManager, ITokenService authenticationService, ITransactionService transactionService, IVideoService videoService)
        {
            _imageService = imageService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _transactionService = transactionService;
            _videoService = videoService;
        }
        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
            IdentityResult result;
            await _transactionService.BeginTransactionAsync();
            try
            {
                AppUserModel user = new()
                {
                    Email = registerDto.Email,
                    Name = registerDto.Name,
                    UserName = registerDto.Nickname,
                    Surname = registerDto.Surname
                };
                if (registerDto.UserType == UserTypeEnum.Creator)
                {
                    user.Money = 0f;
                }
                result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    throw (_userManager.ErrorDescriber.DuplicateEmail(registerDto.Email).Code == result.Errors.First().Code) ?
                        new DuplicateEmailException() : new UserException(result.Errors.First()?.Code);
                }


                result = await _userManager.AddToRoleAsync(user, registerDto.UserType.ToString());
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }

                if (registerDto.AvatarImage is not null)
                {
                    string imagePrefix = @"base64,";
                    if (registerDto.AvatarImage.Contains(imagePrefix))
                    {
                        registerDto.AvatarImage = registerDto.AvatarImage.Split(imagePrefix)[1];
                    }
                    user.ProfilePicture = await _imageService.UploadImageAsync(registerDto.AvatarImage, user.Id.ToString());
                    if (user.ProfilePicture.Url.Length == 0)
                    {
                        throw new UserException("Image upload error");
                    }

                    result = await _userManager.UpdateAsync(user);
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
        public async Task<UserDto> EditUserDataAsync(UpdateUserDto updateUserDto, Guid id)
        {
            AppUserModel user;
            await _transactionService.BeginTransactionAsync();
            try
            {
                IdentityResult result;
                string imagePrefix = @"base64,";

                user = await _userManager.FindByIdAsync(id.ToString());
                user.Name = updateUserDto.Name;
                user.Surname = updateUserDto.Surname;
                user.UserName = updateUserDto.Nickname;

                if (updateUserDto.AvatarImage is not null)
                {
                    if (updateUserDto.AvatarImage.Contains(imagePrefix))
                    {
                        updateUserDto.AvatarImage = updateUserDto.AvatarImage.Split(imagePrefix)[1];
                    }
                    user.ProfilePicture = await _imageService.UploadImageAsync(updateUserDto.AvatarImage, user.Id.ToString());
                    if (user.ProfilePicture.Url.Length == 0)
                    {
                        throw new UserException("Image upload error");
                    }
                }
                else if (user.ProfilePicture is not null)
                {
                    await _imageService.RemoveImageAsync(user.ProfilePicture.FileName);
                    user.ProfilePicture = null;
                }
                result = await _userManager.UpdateAsync(user); // tutaj zapisanie zmian w bazie
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }

                // zmiana roli
                var role = (await _userManager.GetRolesAsync(user)).First() ?? throw new UserException("User has no role");
                var newRole = updateUserDto.UserType.ToString();
                if (newRole != role)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First()?.Code);
                    }

                    result = await _userManager.AddToRoleAsync(user, newRole);
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First()?.Code);
                    }
                    if (newRole == UserTypeEnum.Creator.ToString())
                    {
                        user.Money = 0f;
                        result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            throw new UserException(result.Errors.First()?.Code);
                        }
                    }
                    else if (newRole == UserTypeEnum.Simple.ToString() && role == UserTypeEnum.Creator.ToString())
                    {
                        if (user.Money is null)
                            throw new UserException("Creator doesn't have requered properites");
                        foreach (var video in user.OwnedVideos) // nie wiem czy usuwac video czy co
                            // moze oddzielna metoda w videoService do usuniecia wszystkich
                            await _videoService.RemoveVideoAsync(video.Id, user.Id);

                        user.OwnedVideos.Clear();
                        user.Subscribers.Clear();
                        user.Money = null;
                        result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            throw new UserException(result.Errors.First()?.Code);
                        }
                    }

                }
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();
            return UserMapper.MapUserModelToUserDto(user, (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), (await _userManager.GetRolesAsync(user)).First()));
        }

        public async Task<UserDto> GetUserAsync(Guid id)
        {
            AppUserModel user = await _userManager.FindByIdAsync(id.ToString());
            return user == null
                ? throw new UserNotFoundException()
                : UserMapper.MapUserModelToUserDto(user, (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), (await _userManager.GetRolesAsync(user)).First()));
        }

       

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            AppUserModel user = (await _userManager.FindByEmailAsync(loginDto.Email)) ??
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
            await _transactionService.BeginTransactionAsync();
            try
            {
                AppUserModel user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserException("User doesn't exist"); // UserNotFoundException();

                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new UserException(result.Errors.First()?.Code);
                }
                if (user.ProfilePicture is not null)
                    await _imageService.RemoveImageAsync(user.ProfilePicture.FileName);
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();
        }
    }
}
