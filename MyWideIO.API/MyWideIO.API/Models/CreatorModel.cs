namespace MyVideIO.Models
{
    public class CreatorModel : ViewerModel
    {
        public IEnumerable<VideoModel> OwnedVideos { get; set; }

        public IEnumerable<ViewerModel> Subscribers { get; set; }

        public Decimal Money { get; set; }
    }
}
