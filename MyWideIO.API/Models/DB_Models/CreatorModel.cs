using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace MyWideIO.API.Models.DB_Models
{
    public class CreatorModel : ViewerModel
    {
        public ICollection<VideoModel> OwnedVideos { get; set; }

        public ICollection<ViewerSubscription> Subscribers { get; set; }

        public float Money { get; set; }
    }
}
