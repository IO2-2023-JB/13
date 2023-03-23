using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class ViewerModel : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;

        public IEnumerable<VideoModel> WatchLater { get; set; }


        //creators subs TODO
        //public IEnumerable<CreatorModel> SubscribedTo { get; set; }

        public IEnumerable<VideoModel> LikedVideos { get; set; }

        public IEnumerable<PlaylistModel> Playlists { get; set; }

        public DateTime EndOfBan { get; set; }



    }
}
