
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IVideoService
    {
        public Task<Stream> GetVideo(Guid id);
        public Task<bool> RemoveVideoIfExist(Guid id);
    }
}
