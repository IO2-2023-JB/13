using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IPlaylistRepository
    {
        public Task AddAsync(PlaylistModel playlist);
        public Task UpdateAsync(PlaylistModel playlist);
        public Task RemoveAsync(PlaylistModel playlist);
        public Task RemoveAsync(IEnumerable<PlaylistModel> playlists);
        public Task<PlaylistModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<ICollection<PlaylistModel>> GetUserPlaylistsAsync(Guid userId, CancellationToken cancellationToken = default);
        public IQueryable<PlaylistModel> GetIQuerablePlaylists();
        public Task<List<PlaylistModel>> GetPlaylistsContainingVideo(Guid videoId, CancellationToken cancellationToken = default);

    }
}
