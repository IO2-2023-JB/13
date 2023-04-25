using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace MyWideIO.API.Models.DB_Models
{
    public class CreatorModel : AppUserModel
    {
        public CreatorModel()
        {

        }
        public CreatorModel(AppUserModel user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
            WatchLater = user.WatchLater;
            Subscriptions = user.Subscriptions;
            Likes = user.Likes;
            Playlists = user.Playlists;
            EndOfBan = user.EndOfBan;
            ProfilePicture = user.ProfilePicture;
            AccountBalance = user.AccountBalance;
            SecurityStamp = user.SecurityStamp;
            AccessFailedCount = user.AccessFailedCount;
            ConcurrencyStamp = user.ConcurrencyStamp;
            EmailConfirmed = user.EmailConfirmed;
            LockoutEnabled = user.LockoutEnabled;
            LockoutEnd = user.LockoutEnd;
            TwoFactorEnabled = user.TwoFactorEnabled;
            NormalizedEmail = user.NormalizedEmail;
            PasswordHash = user.PasswordHash;
            NormalizedUserName = user.NormalizedUserName;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
        }
        public ICollection<VideoModel> OwnedVideos { get; set; } = new Collection<VideoModel>();

        public ICollection<ViewerSubscription> Subscribers { get; set; } = new Collection<ViewerSubscription>();

        public float Money { get; set; } = 0f;
    }
}
