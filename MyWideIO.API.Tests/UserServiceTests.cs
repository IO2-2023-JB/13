using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services;
using MyWideIO.API.Services.Interfaces;
using WideIO.API.Models;

namespace MyWideIO.API.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ViewerModel>> _mockUserManager;
        private readonly Mock<SignInManager<ViewerModel>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ViewerModel>>(
                Mock.Of<IUserStore<ViewerModel>>(), null, null, null, null, null, new IdentityErrorDescriber(), null, null);

            _mockSignInManager = new Mock<SignInManager<ViewerModel>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ViewerModel>>(), null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();
            _mockImageService = new Mock<IImageService>();
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
                UserType = UserTypeDto.Viewer,
                AvatarImage = "base64-image"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ViewerModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ViewerModel>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ViewerModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockImageService.Setup(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(("url","filename"));

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
                UserType = UserTypeDto.Viewer,
                AvatarImage = "base64-image"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ViewerModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityErrorDescriber().DuplicateEmail(registerDto.Email)));
            // nie trzeba mockowac wiecej, RegisterUserAsync rzuci wyjatkiem na poczatku i inne metody sie nie wywolaja

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<DuplicateEmailException>();
        }
    }
}
