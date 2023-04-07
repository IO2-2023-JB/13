using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services;
using WideIO.API.Models;
using System.Diagnostics;
using Azure.Storage.Blobs;
using MyWideIO.API.Data;
using Xunit;
using FluentAssertions;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Net;
using MyWideIO.API.Exceptions;

namespace MyWideIO.API.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ViewerModel>> _mockUserManager;
        private readonly Mock<SignInManager<ViewerModel>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IImageService> _mockImageService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ViewerModel>>(
                Mock.Of<IUserStore<ViewerModel>>(), null, null, null, null, null, new IdentityErrorDescriber(), null, null);

            _mockSignInManager = new Mock<SignInManager<ViewerModel>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ViewerModel>>(), null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();
            _mockImageService = new Mock<IImageService>();

            _userService = new UserService(_mockUserManager.Object, _mockImageService.Object, _mockSignInManager.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var registerDto = new RegisterDto // dane nie sa wazne, wazne co zwracaja mockowane metody
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
                .ReturnsAsync("url");

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerDto);

            // Assert
            await act.Should().NotThrowAsync();
        }
        [Fact]
        public async Task RegisterUserAsync_WithAlreadyTakenEmail_ShouldThrowEmailException()
        {
            // Arrange
            var registerDto = new RegisterDto // dane nie sa wazne, wazne co zwracaja mockowane metody
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

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<DuplicateEmailException>();
        }
    }
}
