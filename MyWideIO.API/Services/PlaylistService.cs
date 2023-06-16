using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Mappers;

namespace MyWideIO.API.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IVideoRepository _videoRepository;

        private readonly ITicketRepository _ticketRepository;

        public PlaylistService(IPlaylistRepository playlistRepository, UserManager<AppUserModel> userManager, IVideoRepository videoRepository, ITicketRepository ticketRepository)
        {
            _playlistRepository = playlistRepository;
            _userManager = userManager;
            _videoRepository = videoRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task AddVideoToPlaylistAsync(Guid viewerId, Guid playlistId, Guid videoId)
        {
            PlaylistModel playlist = await _playlistRepository.GetAsync(playlistId, true) ?? throw new PlaylistNotFoundException();
            if (playlist.ViewerId != viewerId)
                throw new ForbiddenException();
            VideoModel video = await _videoRepository.GetAsync(videoId) ?? throw new VideoNotFoundException();
            if (!video.IsVisible && video.CreatorId != viewerId)
                throw new ForbiddenException();
            playlist.VideoPlaylists.Add(new VideoPlaylist
            {
                VideoId = videoId,
                PlaylistId = playlistId
            });
            await _playlistRepository.UpdateAsync(playlist);
        }

        public async Task<CreatePlaylistResponseDto> CreatePlaylistAsync(Guid viewerId, CreatePlaylistRequestDto createPlaylistRequestDto)
        {
            var playlist = new PlaylistModel
            {
                Name = createPlaylistRequestDto.Name,
                IsVisible = createPlaylistRequestDto.Visibility == VisibilityEnum.Public,
                ViewerId = viewerId
            };
            await _playlistRepository.AddAsync(playlist);
            return new CreatePlaylistResponseDto
            {
                Id = playlist.Id
            };
        }

        public async Task<PlaylistDto> EditPlaylistAsync(Guid viewerId, Guid playlistId, PlaylistEditDto playlistEditDto)
        {
            PlaylistModel playlist = await _playlistRepository.GetAsync(playlistId, true) ?? throw new PlaylistNotFoundException();
            if (playlist.ViewerId != viewerId)
                throw new ForbiddenException();
            playlist.Name = playlistEditDto.Name;
            playlist.IsVisible = playlistEditDto.Visibility == VisibilityEnum.Public;
            await _playlistRepository.UpdateAsync(playlist);
            return playlist.ToPlaylistDto();
        }

        public async Task<PlaylistDto> GetPlaylistAsync(Guid viewerId, Guid playlistId)
        {
            PlaylistModel playlist = await _playlistRepository.GetAsync(playlistId, true) ?? throw new PlaylistNotFoundException();
            if (!playlist.IsVisible && playlist.ViewerId != viewerId)
                throw new ForbiddenException();
            return playlist.ToPlaylistDto();
        }

        public async Task<List<PlaylistBaseDto>> GetUserPlaylistsAsync(Guid viewerId, Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is null)
                throw new UserNotFoundException();
            IEnumerable<PlaylistModel> list = await _playlistRepository.GetUserPlaylistsAsync(userId);
            if (viewerId != userId)
                list = list.Where(p => p.IsVisible);
            return list.Select(p => p.ToPlaylistBaseDto()).ToList();
        }

        public async Task RemovePlaylistAsync(Guid userId, Guid playlistId)
        {
            PlaylistModel playlist = await _playlistRepository.GetAsync(playlistId) ?? throw new PlaylistNotFoundException();
            if (playlist.ViewerId != userId)
                throw new ForbiddenException();

            var tickets = await _ticketRepository.GetTargetsTickets(playlistId);
            await _ticketRepository.RemoveAsync(tickets);

            await _playlistRepository.RemoveAsync(playlist);
        }

        public async Task RemoveVideoFromPlaylistAsync(Guid viewerId, Guid playlistId, Guid videoId)
        {
            PlaylistModel playlist = await _playlistRepository.GetAsync(playlistId, true) ?? throw new PlaylistNotFoundException();
            if (playlist.ViewerId != viewerId)
                throw new ForbiddenException();
            if (await _videoRepository.GetAsync(videoId) is null)
                throw new VideoNotFoundException();
            var a = playlist.VideoPlaylists.FirstOrDefault(p => p.VideoId == videoId) ?? throw new VideoNotFoundException();
            playlist.VideoPlaylists.Remove(a);
            await _playlistRepository.UpdateAsync(playlist);
        }

        public async Task<PlaylistDto> GetReccomendedVideosPlaylist(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("No user of given id!");
            List<VideoModel> videos = await _videoRepository.GetUserReccomendationList(userId, 9);
            return new PlaylistDto()
            {
                AuthorId = userId,
                Name = "Favorites",
                Visibility = VisibilityEnum.Private,
                AuthorNickname = user.Name,
                Videos = videos.Select(v => v.ToVideoMetadataDto()).ToList()
            };

        }
    }
}
