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
                Videos = playlist.Videos.Select(VideoMapper.VideoModelToVideoBaseDto).ToList()
            };
            return playlistDto;
        }
        public static PlaylistBaseDto MapPlaylistModelToPlaylistBaseDto(PlaylistModel playlist)
        {
            var playlistBaseDto = new PlaylistBaseDto
            {
                Name = playlist.Name,
                Id = playlist.Id,
                Count = playlist.Videos.Count
            };
            return playlistBaseDto;
        }
    }
}
