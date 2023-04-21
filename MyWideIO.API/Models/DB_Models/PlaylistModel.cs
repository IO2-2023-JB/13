using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWideIO.API.Models.DB_Models
{
    public class PlaylistModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<VideoPlaylist> VideoPlaylists { get; set; }

        public Guid ViewerId { get; set; }
        public AppUserModel Viewer { get; set; }
    }
}