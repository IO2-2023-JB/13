using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Mappers
{
    public static class PlaylistMapper
    {
        public static PlaylistDto ToPlaylistDto(this PlaylistModel playlist)
        {
            var playlistDto = new PlaylistDto
            {
                Name = playlist.Name,
                Visibility = playlist.IsVisible ? VisibilityEnum.Public : VisibilityEnum.Private,
                Videos = playlist.VideoPlaylists.Select(vp => vp.Video.ToVideoMetadataDto()).ToList(),
                AuthorId = playlist.ViewerId,
                AuthorNickname = playlist.Viewer.UserName
            };
            return playlistDto;
        }
        public static PlaylistBaseDto ToPlaylistBaseDto(this PlaylistModel playlist)
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
