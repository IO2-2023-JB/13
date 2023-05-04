namespace MyWideIO.API.BackgroundProcessing
{
    public record class VideoProcessWorkItem(Guid VideoId, Stream VideoFile, string Extension);
    
}
