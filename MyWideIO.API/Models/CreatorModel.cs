<<<<<<< HEAD:MyWideIO.API/MyWideIO.API/Models/CreatorModel.cs
﻿namespace MyVideIO.Models
{
    public class CreatorModel : ViewerModel
    {
        public IEnumerable<VideoModel> OwnedVideos { get; set; }

        public IEnumerable<ViewerModel> Subscribers { get; set; }

        public Decimal Money { get; set; }
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace MyVideIO.Models
{
    public class CreatorModel : ViewerModel
    {
        public IEnumerable<VideoModel> OwnedVideos { get; set; }

        public IEnumerable<ViewerModel> Subscribers { get; set; }

        public Decimal Money { get; set; }
    }
}
>>>>>>> origin/develop:WideIO.API/WideIO.API/Models/CreatorModel.cs
