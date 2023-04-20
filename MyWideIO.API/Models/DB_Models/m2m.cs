using System.ComponentModel.DataAnnotations.Schema;
using WideIO.API.Models;

namespace MyWideIO.API.Models.DB_Models
{
    public class ViewerWatchLater
    {
        public Guid ViewerId { get; set; }
        public ViewerModel Viewer { get; set; }

        public Guid VideoId { get; set; }
        public VideoModel Video { get; set; }
    }

    public class ViewerSubscription
    {
        public Guid ViewerId { get; set; }
        public ViewerModel Viewer { get; set; }

        public Guid CreatorId { get; set; }
        public CreatorModel Creator { get; set; }

        public virtual ICollection<CreatorModel> SubscribedTo { get; set; } = new List<CreatorModel>();
    }

    public class ViewerLike
    {
        public Guid ViewerId { get; set; }
        public ViewerModel Viewer { get; set; }
        public Guid VideoId { get; set; }
        public VideoModel Video { get; set; }
        public ReactionDto Reaction { get; set; }

    }

    public class VideoPlaylist
    {
        public Guid VideoId { get; set; }
        public VideoModel Video { get; set; }

        public Guid PlaylistId { get; set; }
        public PlaylistModel Playlist { get; set; }
    }
}
