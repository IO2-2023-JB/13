using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MyWideIO.API.Data;
using MyWideIO.API.Data.IRepositories;
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

        public async Task<(bool Suceeded, IEnumerable<IdentityError> Errors)> EditUserDataAsync(UserDto userDto)
        {
            IdentityResult result;
            var viewer = await _userManager.FindByIdAsync(userDto.Id.ToString()); // xd guid -> string -> guid

            var emailToken = await _userManager.GenerateChangeEmailTokenAsync(viewer, userDto.Email);
            result = await _userManager.ChangeEmailAsync(viewer, userDto.Email, emailToken);
            if (!result.Succeeded)
                return (false, result.Errors);

            viewer.Name = userDto.Name;
            viewer.Surname = userDto.Surname;
            viewer.UserName = userDto.Nickname;

            result = await _userManager.UpdateAsync(viewer);
            if (!result.Succeeded)
                return (false, result.Errors);

            var role = (await _userManager.GetRolesAsync(viewer)).First();
            string newRole = "";
            switch (userDto.UserType)
            {
                case UserTypeDto.ViewerEnum:
                    if (role != "Viewer")
                        newRole = "Viewer";
                    break;
                case UserTypeDto.CreatorEnum:
                    if (role != "Creator")
                        newRole = "Creator";
                    break;
                case UserTypeDto.AdministratorEnum:
                    if (role != "Admin")
                        newRole = "Admin";
                    break;
            }
            if (newRole.Length > 0)
            {
                result = await _userManager.RemoveFromRoleAsync(viewer, role);
                if (!result.Succeeded)
                    return (false, result.Errors);
                result = await _userManager.AddToRoleAsync(viewer, newRole);
                if (!result.Succeeded)
                    return (false, result.Errors);
            }

            return (true, new List<IdentityError>());
        }

        public async Task<UserDto?> GetUserAsync(Guid id)
        {
            var viewer = await _userManager.FindByIdAsync(id.ToString());
            if (viewer == null)
                return null;
            return new UserDto
            {
                Id = viewer.Id,
                Name = viewer.Name,
                Surname = viewer.Surname,
                Email = viewer.Email,
                Nickname = viewer.UserName,
                UserType = (await _userManager.GetRolesAsync(viewer)).First() switch
                {
                    "Viewer" => UserTypeDto.ViewerEnum,
                    "Creator" => UserTypeDto.CreatorEnum,
                    "Admin" => UserTypeDto.AdministratorEnum,
                },
                AvatarImage = viewer.ProfilePicture == null ? "" : viewer.ProfilePicture
            };
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
            if (!result.Succeeded && modelState != null)
                foreach (var error in result.Errors)
                    modelState.AddModelError(error.Code, error.Description);
            if (!result.Succeeded)
                return false;
            viewer.ProfilePicture = await UploadBlobFile(registerDto.AvatarImage, viewer.Id.ToString() + ".png"); ////  tutaj nie wiem czy nie wstawic backslasha pomiedzy
            await _userManager.UpdateAsync(viewer);

            await _userManager.AddToRoleAsync(viewer, (Random.Shared.Next(3) + 1) switch // na szybko
            {
                1 => "viewer",
                2 => "creator",
                3 => "admin"
            });
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
            if (user != null)
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
                    AvatarImage = user.ProfilePicture == null ? "" : user.ProfilePicture,
                    SubscriptionsCount = user.Subscriptions?.Count,
                };
            }
            return null;
        }

        public async Task<bool> PutUserData(UpdateUserDto dto, Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                //await UploadBlobFile(new MemoryStream(, id.ToString());

                user.Surname = dto.Surname;
                user.Name = dto.Name;
                user.UserName = dto.Nickname;
                user.UserTypeDto = dto.UserType;
                user.ProfilePicture = dto.AvatarImage;
                return true;
            }
            return false;
        }


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

        private string GetBase64Image(Stream bytes)
        {
            using (var rdr = new BinaryReader(bytes))
            {
                byte[] buffer = new byte[bytes.Length];
                rdr.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }


        private async Task<string> GenerateToken(ViewerModel viewer)
        {
            var roles = await _userManager.GetRolesAsync(viewer);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, viewer.Name),
                new Claim(ClaimTypes.Email, viewer.Email),
                new Claim(ClaimTypes.NameIdentifier, viewer.Id.ToString()),
                //new Claim(JwtRegisteredClaimNames.Sub,viewer.Id.ToString()), // podobno id powinno byc w sub
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
