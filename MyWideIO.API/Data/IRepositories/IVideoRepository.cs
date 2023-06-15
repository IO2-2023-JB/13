using MyWideIO.API.Models;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using System.Collections;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IVideoRepository : IRepository<VideoModel>
    {
        public Task<VideoModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<ICollection<VideoModel>> GetUserVideosAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<bool> UserHasVideosAsync(Guid userId, CancellationToken cancellationToken = default);
        public IQueryable<VideoModel> GetIQuerableVideos();
        public Task<List<VideoModel>> GetUserReccomendationList(Guid userId, int n);
        public Task<ICollection<VideoModel>> GetUploadingUploadedProcessingVideos();
    }
}