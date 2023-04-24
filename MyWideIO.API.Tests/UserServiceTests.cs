using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services;
using MyWideIO.API.Services.Interfaces;
using WideIO.API.Models;

namespace MyWideIO.API.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<AppUserModel>> _mockUserManager;
        private readonly Mock<SignInManager<AppUserModel>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IImageStorageService> _mockImageService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<AppUserModel>>(
                Mock.Of<IUserStore<AppUserModel>>(), null, null, null, null, null, new IdentityErrorDescriber(), null, null);

            _mockSignInManager = new Mock<SignInManager<AppUserModel>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<AppUserModel>>(), null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();
            _mockImageService = new Mock<IImageStorageService>();
            _mockTransactionService = new Mock<ITransactionService>();
            _userService = new UserService(_mockUserManager.Object, _mockImageService.Object, _mockSignInManager.Object, _mockTokenService.Object,_mockTransactionService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var registerDto = new RegisterDto // dane nie sa wazne, wazne co zwracaja mockowane metody (chyba)
            {
                Email = "test@example.com",
                Name = "Test",
                Surname = "User",
                Nickname = "testuser",
                Password = "password",
                UserType = UserTypeEnum.Simple,
                AvatarImage = "base64-image"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<AppUserModel>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockImageService.Setup(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ImageModel { Url = "url", FileName = "filename" });

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerDto);

            // Assert
            await act.Should().NotThrowAsync();
        }
        [Fact]
        public async Task RegisterUserAsync_WithAlreadyTakenEmail_ShouldThrowEmailException()
        {
            // Arrange
            var registerDto = new RegisterDto // dane nie sa wazne, wazne co zwracaja mockowane metody (chyba)
            {
                Email = "test@example.com",
                Name = "Test",
                Surname = "User",
                Nickname = "testuser",
                Password = "password",
                UserType = UserTypeEnum.Simple,
                AvatarImage = "base64-image"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityErrorDescriber().DuplicateEmail(registerDto.Email)));
            // nie trzeba mockowac wiecej, RegisterUserAsync rzuci wyjatkiem na poczatku i inne metody sie nie wywolaja

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<DuplicateEmailException>();
        }
    }
}
