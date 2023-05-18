using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWideIO.API.Models.DB_Models
{
    //public class ViewerWatchLater
    //{
    //    public Guid ViewerId { get; set; }
    //    public virtual AppUserModel Viewer { get; set; }

    //    public Guid VideoId { get; set; }
    //    public virtual VideoModel Video { get; set; }
    //}

    public class ViewerSubscription
    {
        public Guid ViewerId { get; set; }
        public virtual AppUserModel Viewer { get; set; }

        public Guid CreatorId { get; set; }
        public virtual AppUserModel Creator { get; set; }

    }

    public class ViewerLike
    {
        public Guid ViewerId { get; set; }
        public virtual AppUserModel Viewer { get; set; }

        public Guid VideoId { get; set; }
        public virtual VideoModel Video { get; set; }
        public ReactionEnum Reaction { get; set; }

    }

    public class VideoPlaylist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Order { get; set; }
        public Guid VideoId { get; set; }
        public virtual VideoModel Video { get; set; }

        public Guid PlaylistId { get; set; }
        public virtual PlaylistModel Playlist { get; set; }
    }
    [Owned]
    public class ImageModel
    {
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
