using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IPlaylistRepository : IRepository<PlaylistModel>
    {

        public Task<ICollection<PlaylistModel>> GetUserPlaylistsAsync(Guid userId, bool includeVideos = false, CancellationToken cancellationToken = default);
        public IQueryable<PlaylistModel> GetIQuerablePlaylists();
        public Task<PlaylistModel?> GetAsync(Guid playlistId, bool includeVideos = false, CancellationToken cancellationToken = default);
        public Task<List<PlaylistModel>> GetPlaylistsContainingVideo(Guid videoId, bool includeVideos = false, CancellationToken cancellationToken = default);

    }
}
