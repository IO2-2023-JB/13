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

namespace MyWideIO.API.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void RegisterUserAsync_ShouldReturn_WhenDataIsCorrect()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ViewerModel>>(MockBehavior.Strict);
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ViewerModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = new Mock<SignInManager<ViewerModel>>(userManagerMock.Object, null, null, null, null, null, null);
            var roleManagerMock = new Mock<RoleManager<UserRole>>(null, null, null, null, null);
            

            var blobServiceClientMock = new Mock<BlobServiceClient>(MockBehavior.Strict);
            blobServiceClientMock.Setup(bs => bs.GetBlobContainerClient(It.IsAny<string>())).Returns(() =>
            {
                var blobContainerClientMock = new Mock<BlobContainerClient>(MockBehavior.Default);
                var responseMock = new Mock<Azure.Response<bool>>(MockBehavior.Strict);
                responseMock.Setup(r => r.Value).Returns(true);
                blobContainerClientMock.Setup(bc => bc.Exists(default)).Returns(() =>
                {
                    var responseMock = new Mock<Azure.Response<bool>>(MockBehavior.Strict);
                    responseMock.Setup(r => r.Value).Returns(true);
                    return responseMock;
                });
                blobContainerClientMock.Setup(bc => bc.GetBlobClient(It.IsAny<string>())).Returns(() =>
                {
                    var blobClient = new Mock<BlobClient>(MockBehavior.Strict);
                    blobClient.Setup(b=>b.UploadAsync(It.IsAny<BinaryData>())).Returns(() =>
                    {
                        var responseMock = new Mock<Azure.Response>(MockBehavior.Strict);
                        return responseMock;
                    });
                });
                return blobContainerClientMock;
            });


            var userService = new UserService(userManagerMock.Object, signInManagerMock.Object, roleManagerMock.Object, blobServiceClientMock.Object);

            var registerDto = new RegisterDto
            {
                Email = "asd@asd.asd",
                Name = "asd",
                Surname = "asd",
                Password = "Password1!",
                UserType = UserTypeDto.Viewer,
                AvatarImage = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="
            };

            // Act
            var task = userService.RegisterUserAsync(registerDto);
            task.Wait();
            // Assert

            task.IsCompletedSuccessfully.Should().BeTrue();
            userManagerMock.VerifyAll();
        }
    }
    //[TestClass]
    //public class UserServiceTests
    //{

    //    [TestMethod]
    //    public async Task RegisterUser_Test()
    //    {
    //        var _userManager = MockUserManager<ViewerModel>(new List<ViewerModel>()).Object;
    //        var _signInManager = MockSignInManager<ViewerModel>(_userManager);
    //        var _blobServiceClient = Mock<BlobServiceClient>()

    //        var userDto = new RegisterDto() { Name = "Nowy", Surname = "NOWY", Email = "aa@b.c", Nickname = "user3", Password = "P@ssw0rd!" };
    //        UserService service = new UserService(_userManager, _signInManager, null);
    //        var result = await service.RegisterUserAsync(userDto, null);

    //        Assert.IsTrue(result);
    //    }

    //    [TestMethod]
    //    public async Task CorrectLogin_Test()
    //    {
    //        var _userManager = MockUserManager<ViewerModel>(new List<ViewerModel>()).Object;
    //        var _signInManager = MockSignInManager<ViewerModel>(_userManager);

    //        var userDto = new RegisterDto() { Name = "Nowy", Surname = "NOWY", Email = "aa@b.c", Nickname = "user3", Password = "P@ssw0rd!" };
    //        UserService service = new UserService(_userManager, _signInManager, null);
    //        var result = await service.RegisterUserAsync(userDto, null);

    //        Assert.IsTrue(result);

    //        var logResult = await service.LoginUserAsync(new LoginDto() { Email = "aa@b.c", Password = "P@ssw0rd!" });

    //        Assert.IsNotNull(logResult);
    //    }

    //    [TestMethod]
    //    public async Task InvalidPasswordLogin_Test()
    //    {
    //        var _userManager = MockUserManager<ViewerModel>(new List<ViewerModel>()).Object;
    //        var _signInManager = MockSignInManager<ViewerModel>(_userManager);

    //        var userDto = new RegisterDto() { Name = "Nowy", Surname = "NOWY", Email = "aa@b.c", Nickname = "user3", Password = "P@ssw0rd!" };
    //        UserService service = new UserService(_userManager, _signInManager, null);
    //        var result = await service.RegisterUserAsync(userDto, null);

    //        Assert.IsTrue(result);

    //        var logResult = await service.LoginUserAsync(new LoginDto() { Email = "aa@b.c", Password = "WRONG" });

    //        Assert.IsNotNull(logResult);
    //    }

    //    [TestMethod]
    //    public async Task DeleteUser_Test()
    //    {
    //        var _userManager = MockUserManager<ViewerModel>(new List<ViewerModel>()).Object;
    //        var _signInManager = MockSignInManager<ViewerModel>(_userManager);

    //        var userDto = new RegisterDto() { Name = "Nowy", Surname = "NOWY", Email = "aa@b.c", Nickname = "user3", Password = "P@ssw0rd!" };
    //        UserService service = new UserService(_userManager, _signInManager, null);
    //        var result = await service.RegisterUserAsync(userDto, null);

    //        Assert.IsTrue(result);
    //        Assert.AreEqual(0, _userManager.Users.Count());

    //        //Guid userId = _signInManager.UserManager.Users.Single().Id;
    //        //result = await service.DeleteUserAsync(userId);
    //        //Assert.IsTrue(result);
    //        //Assert.AreEqual(0, _userManager.Users.Count());
    //    }

    //    // help function

    //    private static List<ViewerModel> GetUsers() => new List<ViewerModel>
    //    {
    //        new ViewerModel() { Name="Jan",Surname="Kowalski",Email="a1@b.c",UserName="user1", PasswordHash="" },
    //        new ViewerModel() { Name="Eu",Surname="Geniusz",Email="a2@b.c",UserName="user2" }
    //    };

    //    private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
    //    {
    //        var store = new Mock<IUserStore<TUser>>();
    //        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    //        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
    //        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

    //        mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
    //        mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
    //        mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

    //        return mgr;
    //    }

    //    public static SignInManager<TUser> MockSignInManager<TUser>(UserManager<TUser> manager) where TUser : class
    //        => new SignInManager<TUser>(
    //                manager,
    //                new Mock<IHttpContextAccessor>().Object,
    //                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
    //                new Mock<IOptions<IdentityOptions>>().Object,
    //                new Mock<ILogger<SignInManager<TUser>>>().Object,
    //                new Mock<IAuthenticationSchemeProvider>().Object,
    //                null
    //            );

    //}
}
