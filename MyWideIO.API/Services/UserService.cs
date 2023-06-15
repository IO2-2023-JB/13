using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;
using System.Text;

namespace MyWideIO.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly ITokenService _authenticationService;
        private readonly ITransactionService _transactionService;
        private readonly IImageStorageService _imageService;
        private readonly IVideoRepository _videoRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IVideoService _videoService;

        public UserService(
            UserManager<AppUserModel> userManager,
            IImageStorageService imageService,
            SignInManager<AppUserModel> signInManager,
            ITokenService authenticationService,
            ITransactionService transactionService,
            IVideoRepository videoRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            IPlaylistRepository playlistRepository,
            ISubscriptionRepository subscriptionRepository,
            ITicketRepository ticketRepository,
            IVideoService videoService
            )
        {
            _imageService = imageService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _transactionService = transactionService;
            _videoRepository = videoRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _playlistRepository = playlistRepository;
            _subscriptionRepository = subscriptionRepository;
            _ticketRepository = ticketRepository;
            _videoService = videoService;
        }
        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
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
                var result = await _userManager.CreateAsync(user, registerDto.Password);
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

                // zmiana roli
                var role = (await _userManager.GetRolesAsync(user)).First() ?? throw new UserException("User has no role");
                if (role == UserTypeEnum.Administrator.ToString())
                {
                    throw new UserException("Admin cannot be changed to other role");
                }
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
                        var videos = await _videoRepository.GetUserVideosAsync(id);
                        foreach (var video in videos)
                            await _videoService.RemoveVideoAsync(video.Id, id);

                        // remove subscriptions to creator
                        var subscriptions = await _subscriptionRepository.GetSubscriptionsToCreator(id);
                        await _subscriptionRepository.RemoveAsync(subscriptions);

                        result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            throw new UserException(result.Errors.First()?.Code);
                        }
                    }

                }
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
            }
            catch
            {
                await _transactionService.RollbackAsync();
                throw;
            }
            await _transactionService.CommitAsync();
            return user.ToUserDto((UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), (await _userManager.GetRolesAsync(user)).First()));
        }

        public async Task<UserDto> GetUserAsync(Guid id, Guid askerId)
        {
            AppUserModel user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException();

            return user.ToUserDto((UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), (await _userManager.GetRolesAsync(user)).First()), askerId == id);
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
                AppUserModel user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException();

                var role = (await _userManager.GetRolesAsync(user)).First();
                if (role == UserTypeEnum.Creator.ToString())
                {
                    var videos = await _videoRepository.GetUserVideosAsync(id);
                    foreach (var video in videos)
                        await _videoService.RemoveVideoAsync(video.Id, id);

                    // remove subscriptions
                    var subscriptionsToCreator = await _subscriptionRepository.GetSubscriptionsToCreator(id);
                    await _subscriptionRepository.RemoveAsync(subscriptionsToCreator);
                }

                // remove all likes
                var likes = await _likeRepository.GetUserLikesAsync(id);
                foreach (var like in likes)
                {
                    VideoModel video;
                    video = await _videoRepository.GetAsync(like.VideoId) ?? throw new VideoException("video was removed, while trying to remove its likes");

                    switch (like.Reaction)
                    {
                        case ReactionEnum.Positive:
                            video.PositiveReactions--;
                            break;
                        case ReactionEnum.Negative:
                            video.NegativeReactions--;
                            break;
                        default:
                            break;
                    }
                    await _videoRepository.UpdateAsync(video, false);
                }
                await _videoRepository.SaveChangesAsync();
                await _likeRepository.RemoveAsync(likes);


                // remove all comments
                var comments = await _commentRepository.GetUserCommentsAsync(id);
                await _commentRepository.RemoveAsync(comments);

                // remove all playlists
                var playlists = await _playlistRepository.GetUserPlaylistsAsync(id);
                await _playlistRepository.RemoveAsync(playlists);

                // remove user's subscriptions
                var subscriptions = await _subscriptionRepository.GetViewersSubscriptionsAsync(id);
                foreach (var subscription in subscriptions)
                {
                    var creator = await _userManager.FindByIdAsync(subscription.CreatorId.ToString());
                    creator.SubscribersAmount--;
                    await _userManager.UpdateAsync(creator);
                }
                await _subscriptionRepository.RemoveAsync(subscriptions);

                // remove tickets
                var tickets = await _ticketRepository.GetTargetsTickets(id); // tickets where user is target
                tickets.AddRange(await _ticketRepository.GetUserTicketsAsync(id)); // tickets where user is sender

                await _ticketRepository.RemoveAsync(tickets);

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
