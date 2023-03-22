<<<<<<< HEAD:MyWideIO.API/MyWideIO.API/Models/ViewerModel.cs
﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class ViewerModel : IdentityUser<int>
    {
        
        [Required] 
        public string Nick { get; set; } = string.Empty;

        public IEnumerable<VideoModel> WatchLater { get; set; }

        //creators subs TODO
        public IEnumerable<CreatorModel> SubscribedTo { get; set; }

        public IEnumerable<VideoModel> LikedVideos { get; set; }

        public IEnumerable<PlaylistModel> Playlists { get; set; }

        public DateTime EndOfBan { get; set; }



    }
}
=======
﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class ViewerModel : IdentityUser<int>
    {
        
        [Required] 
        public string Nick { get; set; } = string.Empty;

        public IEnumerable<VideoModel> WatchLater { get; set; }

        //creators subs TODO
        public IEnumerable<CreatorModel> SubscribedTo { get; set; }

        public IEnumerable<VideoModel> LikedVideos { get; set; }

        public IEnumerable<PlaylistModel> Playlists { get; set; }

        public DateTime EndOfBan { get; set; }



    }
}
>>>>>>> origin/develop:WideIO.API/WideIO.API/Models/ViewerModel.cs
