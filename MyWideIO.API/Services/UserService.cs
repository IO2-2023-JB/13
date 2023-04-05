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
using Azure.Storage.Blobs;
using System.Reflection.PortableExecutable;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ViewerModel> _userManager;
        private readonly SignInManager<ViewerModel> _signInManager;
        private readonly RoleManager<UserRole> _roleManager;

        // BLOBA MOZNA ZROBIC DO TEGO JAK NIE BEDZIE 
        // DZIALALO ODKLADANIE  W BAZIE

        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;
        //private string blobServiceConnectionString = "https://videioblob.blob.core.windows.net/blob1"; /* TODO */
        private readonly string containerName = "blob1";

        public UserService(UserManager<ViewerModel> userManager, SignInManager<ViewerModel> signInManager, RoleManager<UserRole> roleManager, BlobServiceClient blobServiceClient)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
            _roleManager = roleManager;
        }

        public async Task<UserDto> EditUserDataAsync(UpdateUserDto updateUserDto, Guid id)
        {
            IdentityResult result;
            var viewer = await _userManager.FindByIdAsync(id.ToString());

            viewer.Name = updateUserDto.Name;
            viewer.Surname = updateUserDto.Surname;
            viewer.UserName = updateUserDto.Nickname;
            viewer.ProfilePicture = await UploadBlobFile(updateUserDto.AvatarImage, viewer.Id.ToString() + ".png");
            if (viewer.ProfilePicture.Length == 0)
                throw new UserException("Azure Blob error");

            result = await _userManager.UpdateAsync(viewer);
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);

            var role = (await _userManager.GetRolesAsync(viewer)).First();
            string newRole = "";
            switch (updateUserDto.UserType)
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
            return await userModelToDtoConverter(viewer);
        }

        public async Task<UserDto> GetUserAsync(Guid id)
        {
            var viewer = await _userManager.FindByIdAsync(id.ToString());
            return viewer == null
                ? throw new UserNotFoundException() // dziwne ze sie kompiluje
                : await userModelToDtoConverter(viewer);
        }

        private async Task<UserDto> userModelToDtoConverter(ViewerModel viewer)
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
                AvatarImage = viewer.ProfilePicture == null ? "" : viewer.ProfilePicture
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
                Surname = registerDto.Surname

            };
            result = await _userManager.CreateAsync(viewer, registerDto.Password);
            if (!result.Succeeded)
                throw (_userManager.ErrorDescriber.DuplicateEmail(registerDto.Email).Code == result.Errors.First().Code) ?
                     new DuplicateEmailException() : new UserException(result.Errors.First()?.Code);
            viewer.ProfilePicture = await UploadBlobFile(registerDto.AvatarImage, viewer.Id.ToString() + ".png"); ////  tutaj nie wiem czy nie wstawic backslasha pomiedzy
            await _userManager.UpdateAsync(viewer);
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);
            await _userManager.AddToRoleAsync(viewer, registerDto.UserType switch // na szybko
            {
                UserTypeDto.Viewer => "viewer",
                UserTypeDto.Creator => "creator",
                UserTypeDto.Administrator => "admin",
                _ => throw new UserException("Invalid user type")
            });
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new UserException(result.Errors.First()?.Code);
        }

        //public async Task<UserDto?> GetUser(Guid id)
        //{
        //    var user = await _userManager.FindByIdAsync(id.ToString());
        //    if (user != null)
        //    {
        //        return new UserDto()
        //        {
        //            Id = id,
        //            Email = user.Email,
        //            Nickname = user.UserName,
        //            Name = user.Name,
        //            Surname = user.Surname,
        //            AccountBalance = user.AccountBalance,
        //            UserType = user.UserTypeDto,
        //            AvatarImage = user.ProfilePicture == null ? "" : user.ProfilePicture,
        //            SubscriptionsCount = user.Subscriptions?.Count,
        //        };
        //    }
        //    return null;
        //}

        //public async Task<bool> PutUserData(UpdateUserDto dto, Guid id)
        //{
        //    var user = await _userManager.FindByIdAsync(id.ToString());
        //    if (user != null)
        //    {
        //        //await UploadBlobFile(new MemoryStream(, id.ToString());

        //        user.Surname = dto.Surname;
        //        user.Name = dto.Name;
        //        user.UserName = dto.Nickname;
        //        user.UserTypeDto = dto.UserType;
        //        user.ProfilePicture = dto.AvatarImage;
        //        return true;
        //    }
        //    return false;
        //}


        private async Task<string> UploadBlobFile(string base64photo, string fileName)
        {
            byte[] buffer = Convert.FromBase64String(base64photo);

            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);



            BinaryData binaryData = new BinaryData(buffer);

            await blobClient.UploadAsync(binaryData, true);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = "image/png"
            });



            return blobClient.Uri.AbsoluteUri;
        }


        //private async Task<byte[]> GetBlobFile(string fileName)
        //{
        //    BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
        //    if (await blobClient.ExistsAsync())
        //    {
        //        var response = await blobClient.DownloadAsync();
        //        using (var rd = new BinaryReader(response.Value.Content))
        //        {
        //            long size = rd.BaseStream.Length;
        //            byte[] bts = new byte[size];
        //            rd.Read(bts, 0, (int)size);
        //            return bts;
        //        }
        //    }
        //    return null;
        //}

        //private string GetBase64Image(Stream bytes)
        //{
        //    using (var rdr = new BinaryReader(bytes))
        //    {
        //        byte[] buffer = new byte[bytes.Length];
        //        rdr.Read(buffer, 0, buffer.Length);
        //        return Convert.ToBase64String(buffer);
        //    }
        //}


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
