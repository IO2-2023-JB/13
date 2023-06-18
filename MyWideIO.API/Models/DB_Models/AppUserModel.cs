using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MyWideIO.API.Models.DB_Models
{


    public class AppUserModel : IdentityUser<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;

        public virtual ICollection<VideoModel> WatchLater { get; set; } = new Collection<VideoModel>();
        public virtual ICollection<ViewerSubscription> Subscriptions { get; set; } = new Collection<ViewerSubscription>();

        public virtual ICollection<ViewerLike> Likes { get; set; } = new Collection<ViewerLike>();
        public virtual ICollection<PlaylistModel> Playlists { get; set; } = new Collection<PlaylistModel>();
        public virtual ImageModel? ProfilePicture { get; set; }
        public decimal AccountBalance { get; set; }

        // CREATOR
        public virtual ICollection<VideoModel> OwnedVideos { get; set; } = new Collection<VideoModel>();

        public virtual ICollection<ViewerSubscription> Subscribers { get; set; } = new Collection<ViewerSubscription>();

        public float? Money { get; set; }
        public int SubscribersAmount { get; set; }

        // public UserTypeEnum UserTypeDto { get; set; } = UserTypeEnum.Simple;
    }
}
