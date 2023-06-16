using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Services.Interfaces
{
    public interface IPlaylistService
    {
        public Task RemovePlaylistAsync(Guid viewerId, Guid playlistId);
        public Task<CreatePlaylistResponseDto> CreatePlaylistAsync(Guid viewerId, CreatePlaylistRequestDto createPlaylistRequestDto);
        public Task<PlaylistDto> EditPlaylistAsync(Guid viewerId, Guid playlistId, PlaylistEditDto playlistEditDto);
        public Task<PlaylistDto> GetPlaylistAsync(Guid viewerId, Guid playlistId);
        public Task<List<PlaylistBaseDto>> GetUserPlaylistsAsync(Guid viewerId, Guid userId); // viewerId pyta or playlisty usera o id userId, potrzebne do prywatnych playlist
        public Task AddVideoToPlaylistAsync(Guid viewerId, Guid playlistId, Guid videoId);
        public Task RemoveVideoFromPlaylistAsync(Guid viewerId, Guid playlistId, Guid videoId);
        public Task<PlaylistDto> GetReccomendedVideosPlaylist(Guid userId);
    }
}
