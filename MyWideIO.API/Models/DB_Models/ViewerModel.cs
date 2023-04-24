using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWideIO.API.Models.DB_Models
{


    public class AppUserModel : IdentityUser<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;

        public ICollection<ViewerWatchLater> WatchLater { get; set; }
        public ICollection<ViewerSubscription> Subscriptions { get; set; }

        public ICollection<ViewerLike> Likes { get; set; }
        public ICollection<PlaylistModel> Playlists { get; set; }
        public DateTime EndOfBan { get; set; }
        public ImageModel? ProfilePicture { get; set; }
        public decimal? AccountBalance { get; set; }
        public UserTypeEnum UserTypeDto { get; set; }
    }
}
