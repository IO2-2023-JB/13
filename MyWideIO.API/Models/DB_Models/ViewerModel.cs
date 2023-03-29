using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyWideIO.API.Models.DB_Models
{
    public class ViewerModel : IdentityUser<Guid>
    {

        public string Nick { get; set; } = string.Empty;

        public ICollection<ViewerWatchLater> WatchLater { get; set; }
        public ICollection<ViewerSubscription> Subscriptions { get; set; }

        public ICollection<ViewerLike> Likes { get; set; }
        public ICollection<PlaylistModel> Playlists { get; set; }
        public DateTime EndOfBan { get; set; }
    }
}
