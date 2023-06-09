using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyWideIO.API.Controllers;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Services;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Data.IRepositories;
using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Models.DB_Models;
using Microsoft.AspNetCore.Http;
using MyWideIO.API.Data;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace MyWideIO.API.Tests
{

    //public static class MockExtensions
    //{
    //    public static void SetupIQueryable<T>(this Mock<T> mock, IQueryable queryable)
    //        where T : class, IQueryable
    //    {
    //        mock.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
    //        mock.Setup(r => r.Provider).Returns(queryable.Provider);
    //        mock.Setup(r => r.ElementType).Returns(queryable.ElementType);
    //        mock.Setup(r => r.Expression).Returns(queryable.Expression);
    //    }
    //}

    //public class CommentControllerTests
    //{
    //    private Controllers.CommentApiController controller;
    //    private readonly Mock<UserManager<AppUserModel>> _mockUserManager;
    //    private readonly Mock<SignInManager<AppUserModel>> _mockSignInManager;
    //    private readonly Mock<ITokenService> _mockTokenService;
    //    private readonly Mock<IImageStorageService> _mockImageService;
    //    private readonly Mock<ITransactionService> _mockTransactionService;
    //    private readonly Mock<IVideoService> _mockVideoService;
    //    private Mock<UserService> userService;
    //    private Mock<ICommentService> commentService;
    //    private Mock<ApplicationDbContext> _appContext;
    //    private Mock<CommentRepository> commentRepository;

    //    private Guid v1ID = Guid.NewGuid();
    //    private Guid userID = Guid.NewGuid();
    //    private List<AppUserModel> userData;
    //    private List<VideoModel> videoData;
    //    private List<CommentModel> commentData;

    //    public CommentControllerTests()
    //    {
    //        var userSet = new Mock<DbSet<AppUserModel>>();
    //        var videoSet = new Mock<DbSet<VideoModel>>();
    //        var commentSet = new Mock<DbSet<CommentModel>>();

    //        userData = new List<AppUserModel>{ new AppUserModel()
    //        {
    //            Id = userID
    //        }};
            
    //        videoData = new List<VideoModel>{ new VideoModel()
    //        {
    //            Id = v1ID,
    //            CreatorId = new Guid(),
    //            Description = "Description",
    //            Title = "Title"
    //        }};

    //        commentData = new List<CommentModel>();

    //        userSet.SetupIQueryable(userData.AsQueryable());
    //        videoSet.SetupIQueryable(videoData.AsQueryable());
    //        commentSet.SetupIQueryable(commentData.AsQueryable());

    //        _appContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
    //        _appContext.Object.Users = userSet.Object;
    //        _appContext.Object.Videos = videoSet.Object;
    //        _appContext.Object.Comments = commentSet.Object;

    //        commentRepository = new Mock<CommentRepository>(_appContext.Object);

    //        _mockUserManager = new Mock<UserManager<AppUserModel>>(
    //            Mock.Of<IUserStore<AppUserModel>>(), null, null, null, null, null, new IdentityErrorDescriber(), null, null);

    //        _mockSignInManager = new Mock<SignInManager<AppUserModel>>(
    //            _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<AppUserModel>>(), null, null, null, null);

    //        _mockTokenService = new Mock<ITokenService>();
    //        _mockImageService = new Mock<IImageStorageService>();
    //        _mockTransactionService = new Mock<ITransactionService>();
    //        _mockVideoService = new Mock<IVideoService>();

    //        userService = new Mock<UserService>(_mockUserManager.Object, _mockImageService.Object, _mockSignInManager.Object, 
    //            _mockTokenService.Object, _mockTransactionService.Object, _mockVideoService.Object);


    //        var serv = new Mock<CommentService>(commentRepository.Object, userService.Object);
    //        commentService = new Mock<ICommentService>();
    //        controller = new CommentApiController(commentService.Object);
    //        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    //        {
    //            new Claim(ClaimTypes.Name, "example name"),
    //            new Claim(ClaimTypes.NameIdentifier, userID.ToString()),
    //            new Claim("custom-claim", "example claim value"),
    //        }, "mock"));
    //        controller.ControllerContext = new ControllerContext()
    //        {
    //            HttpContext = new DefaultHttpContext() { User = user }
    //        };
    //    }

    //    [Fact]
    //    public async void TestAddCommentOk()
    //    {
    //        IActionResult result = await controller.AddCommentToVideo(v1ID, "KOMENTARZ2");
    //        var okRes = result as OkResult;
    //        Assert.NotNull(okRes);
    //        Assert.Equal(200, okRes.StatusCode);
    //    }

    //    [Fact]
    //    public async void TestGetCommentsOk()
    //    {
    //        var commentsResult = await controller.GetComments(v1ID);
    //        Assert.NotNull(commentsResult);
    //        Assert.NotNull(commentsResult.Result);
    //        var okR = commentsResult.Result as OkObjectResult;
    //        Assert.NotNull(okR);
    //        Assert.Equal(200, okR.StatusCode);
    //    }

    //    [Fact]
    //    public async void TestGetResponseCommentOk()
    //    {
    //        var commentResult = await controller.GetResponseData(Guid.NewGuid());
    //        Assert.NotNull(commentResult);
    //        Assert.NotNull(commentResult.Result);
    //        var okR = commentResult.Result as OkObjectResult;
    //        Assert.NotNull(okR);
    //        Assert.Equal(200, okR.StatusCode);
    //    }

    //    [Fact]
    //    public async void TestDeleteCommentOk()
    //    {
    //        var result = await controller.DeleteComment(Guid.NewGuid());
    //        var okRes = result as OkResult;
    //        Assert.NotNull(okRes);
    //        Assert.Equal(200, okRes.StatusCode);
    //    }

    //    [Fact]
    //    public async void TestAddCommentResponsesOk()
    //    {
    //        var result = await controller.AddResponseToComment(Guid.NewGuid(), "Test commment");
    //        var okRes = result as OkResult;
    //        Assert.NotNull(okRes);
    //        Assert.Equal(200, okRes.StatusCode);
    //    }



    //}
}
