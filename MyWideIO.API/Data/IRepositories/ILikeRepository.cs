using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ILikeRepository : IRepository<ViewerLike>
    {
        public Task<ICollection<ViewerLike>> GetUserLikesAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<ICollection<ViewerLike>> GetVideoLikesAsync(Guid videoId, CancellationToken cancellationToken = default);
        public Task<ViewerLike?> GetUserLikeOfVideoAsync(Guid userId, Guid videoId, CancellationToken cancellationToken = default);
        
        

    }
}
