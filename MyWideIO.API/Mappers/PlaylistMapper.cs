using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Mappers
{
    public static class PlaylistMapper
    {
        public static PlaylistDto MapPlaylistModelToPlaylistDto(PlaylistModel playlist)
        {
            var playlistDto = new PlaylistDto
            {
                Name = playlist.Name,
                Visibility = playlist.IsVisible ? VisibilityEnum.Public : VisibilityEnum.Private,
                Videos = playlist.VideoPlaylists.Select(vp => VideoMapper.VideoModelToVideoMetadataDto(vp.Video)).ToList()
            };
            return playlistDto;
        }
        public static PlaylistBaseDto MapPlaylistModelToPlaylistBaseDto(PlaylistModel playlist)
        {
            var playlistBaseDto = new PlaylistBaseDto
            {
                Name = playlist.Name,
                Id = playlist.Id,
                Visibility = playlist.IsVisible ? VisibilityEnum.Public : VisibilityEnum.Private
            };
            return playlistBaseDto;
        }
    }
}
