using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.IRepositories
{
    public interface IPlaylistRepository
    {
        public Task AddPlaylistAsync(PlaylistModel playlist);
        public Task UpdatePlaylistAsync(PlaylistModel playlist);
        public Task RemovePlaylistAsync(PlaylistModel playlist);
        public Task<PlaylistModel?> GetPlaylistAsync(Guid id);
        public Task<ICollection<PlaylistModel>> GetUserPlaylists(Guid userId);
    }
}
