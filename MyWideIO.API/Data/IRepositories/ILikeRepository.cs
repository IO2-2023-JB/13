using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ILikeRepository
    {
        public Task UpdateAsync(ViewerLike like);
        public Task<ICollection<ViewerLike>> GetUserLikesAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<ICollection<ViewerLike>> GetVideoLikesAsync(Guid videoId, CancellationToken cancellationToken = default);
        public Task<ViewerLike?> GetUserLikeOfVideoAsync(Guid userId, Guid videoId, CancellationToken cancellationToken = default); 
        public Task AddAsync(ViewerLike like);
        public Task DeleteAsync(ViewerLike like);
        public Task DeleteAsync(IEnumerable<ViewerLike> likes);
    }
}
