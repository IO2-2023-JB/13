using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MyWideIO.API.Models.DB_Models
{


    public class AppUserModel : IdentityUser<Guid>
    {
        public AppUserModel()
        {

        }
        public AppUserModel(CreatorModel creator)
        {
            Id = creator.Id;
            UserName = creator.UserName;
            Name = creator.Name;
            Surname = creator.Surname;
            Email = creator.Email;
            WatchLater = creator.WatchLater;
            Subscriptions = creator.Subscriptions;
            Likes = creator.Likes;
            Playlists = creator.Playlists;
            EndOfBan = creator.EndOfBan;
            ProfilePicture = creator.ProfilePicture;
            AccountBalance = creator.AccountBalance;
            SecurityStamp = creator.SecurityStamp;
            AccessFailedCount = creator.AccessFailedCount;
            ConcurrencyStamp = creator.ConcurrencyStamp;
            EmailConfirmed = creator.EmailConfirmed;
            LockoutEnabled = creator.LockoutEnabled;
            LockoutEnd = creator.LockoutEnd;
            TwoFactorEnabled = creator.TwoFactorEnabled;
            NormalizedEmail = creator.NormalizedEmail;
            PasswordHash = creator.PasswordHash;
            NormalizedUserName = creator.NormalizedUserName;
            PhoneNumber = creator.PhoneNumber;
            PhoneNumberConfirmed = creator.PhoneNumberConfirmed;
        }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;

        public ICollection<ViewerWatchLater> WatchLater { get; set; } = new Collection<ViewerWatchLater>();
        public ICollection<ViewerSubscription> Subscriptions { get; set; } = new Collection<ViewerSubscription>();

        public ICollection<ViewerLike> Likes { get; set; } = new Collection<ViewerLike>();
        public ICollection<PlaylistModel> Playlists { get; set; } = new Collection<PlaylistModel>();
        public DateTime EndOfBan { get; set; }
        public ImageModel? ProfilePicture { get; set; }
        public decimal? AccountBalance { get; set; }
        // public UserTypeEnum UserTypeDto { get; set; } = UserTypeEnum.Simple;
    }
}
