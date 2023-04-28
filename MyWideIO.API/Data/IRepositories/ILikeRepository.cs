using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface ILikeRepository
    {
        public Task UpdateAsync(ViewerLike like);
        public Task<ICollection<ViewerLike>> GetUserLikesAsync(Guid userId);
        public Task<ICollection<ViewerLike>> GetVideoLikesAsync(Guid videoId);
        public Task<ViewerLike?> GetUserLikeOfVideoAsync(Guid userId, Guid videoId); 
        public Task AddAsync(ViewerLike like);
        public Task DeleteAsync(ViewerLike like);
    }
}
