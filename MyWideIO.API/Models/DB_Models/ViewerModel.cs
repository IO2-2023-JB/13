using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using WideIO.API.Models;

namespace MyWideIO.API.Models.DB_Models
{


    public class ViewerModel : IdentityUser<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;

        public string Nick { get; set; } = string.Empty;

        public ICollection<ViewerWatchLater> WatchLater { get; set; }
        public ICollection<ViewerSubscription> Subscriptions { get; set; }

        public ICollection<ViewerLike> Likes { get; set; }
        public ICollection<PlaylistModel> Playlists { get; set; }
        public DateTime EndOfBan { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? AccountBalance { get; set; }
        public UserTypeDto UserTypeDto { get; set; }
    }
}
