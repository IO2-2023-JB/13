namespace MyWideIO.API.Models.DB_Models
{
    public class PlaylistModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<VideoPlaylist> VideoPlaylists { get; set; }

        public Guid ViewerId { get; set; }
        public virtual AppUserModel Viewer { get; set; }
        public bool IsVisible { get; set; }
    }
}