using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WideIO.API.Models;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Data.IRepositories;
using Azure.Storage.Blobs;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ViewerModel> _userManager;
        private readonly SignInManager<ViewerModel> _signInManager;
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;
        private string blobServiceConnectionString = ""; /* TODO */

        public UserService(UserManager<ViewerModel> userManager, SignInManager<ViewerModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blobServiceClient = new BlobServiceClient(blobServiceConnectionString);
            _blobContainerClient = _blobServiceClient.CreateBlobContainer("container-" + Guid.NewGuid());
        }

        public async Task<bool> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
            return result.Succeeded;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto, ModelStateDictionary modelState)
        {
            ViewerModel viewer = new ViewerModel
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserName = registerDto.Nickname,
                Surname = registerDto.Surname,
                //ProfilePicture = (await UploadBlobFile(registerDto.AvatarImage)) // Nie możemy wstawić tutaj zdjecia bo nie znamy id uzytkownika
            };
            var result = await _userManager.CreateAsync(viewer, registerDto.Password);
            if (!result.Succeeded && modelState != null)
                foreach (var error in result.Errors)
                    modelState.AddModelError(error.Code, error.Description);
            else if(result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(viewer.Email);
                user.ProfilePicture = (await UploadBlobFile(registerDto.AvatarImage, user.Id.ToString())); ////  tutaj nie wiem czy nie wstawic backslasha pomiedzy
            }
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<UserDto?> GetUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user != null)
            {
                return new UserDto()
                {
                    Id = id,
                    Email = user.Email,
                    Nickname = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    AccountBalance = user.AccountBalance,
                    UserType = user.UserTypeDto,
                    AvatarImage = blobServiceConnectionString + user.Id.ToString(),
                    SubscriptionsCount = user.Subscriptions.Count
                };
            }
            return null;
        }

        public async Task<bool> PutUserData(UpdateUserDto dto, Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await UploadBlobFile(new MemoryStream(Convert.FromBase64String(dto.AvatarImage)), id.ToString());

                user.Surname= dto.Surname;
                user.Name = dto.Name;
                user.UserName = dto.Nickname;
                user.UserTypeDto = dto.UserType;
                return true;
            }
            return false;
        }


        private async Task<string> UploadBlobFile(Stream bytes, string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

            BinaryReader reader = new BinaryReader(bytes);

            byte[] buffer = new byte[bytes.Length];

            reader.Read(buffer, 0, buffer.Length);

            BinaryData binaryData = new BinaryData(buffer);

            await blobClient.UploadAsync(binaryData, true);

            return fileName;
        }

        private async Task<byte[]> GetBlobFile(string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using (var rd = new BinaryReader(response.Value.Content))
                {
                    long size = rd.BaseStream.Length;
                    byte[] bts = new byte[size];
                    rd.Read(bts, 0, (int)size);
                    return bts;
                }
            }
            return null;
        }

    }
}
