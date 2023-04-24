namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoStorageService
    {
        public Task<Stream> GetVideoFileAsync(Guid id);
        // public Stream GetVideoStream(Guid id);
        public Task RemoveVideoFileAsync(Guid id);
        public Task UploadVideoFileAsync(Guid id, Stream stream);
    }
}
