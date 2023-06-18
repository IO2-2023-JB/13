namespace MyWideIO.API.BackgroundProcessing
{
    public record class VideoProcessWorkItem(Guid VideoId, string fileName);
    
}
