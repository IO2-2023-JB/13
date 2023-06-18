namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoStorageService
    {
        public Task<Stream> GetVideoFileAsync(Guid id, CancellationToken cancellationToken = default);
        // public Stream GetVideoFile(Guid id);
        public Task RemoveVideoFileAsync(Guid id);
        public Task UploadVideoFileAsync(Guid id, Stream stream, CancellationToken cancellationToken = default);
        // public Task<int> GetVideoDurationAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
