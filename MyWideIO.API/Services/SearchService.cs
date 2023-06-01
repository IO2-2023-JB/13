using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Mappers;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Services
{
    public class SearchService : ISearchService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IVideoRepository _videoRepository;
        private readonly IPlaylistRepository _playlistRepository;

        public SearchService(UserManager<AppUserModel> userManager, IVideoRepository videoRepository, IPlaylistRepository playlistRepository)
        {
            _userManager = userManager;
            _videoRepository = videoRepository;
            _playlistRepository = playlistRepository;
        }
        public async Task<SearchResultsDto> GetSearchResultsAsync(string query, SortingTypesEnum sortingCriteria, SortingDirectionsEnum sortingType, DateTime? beginDate, DateTime? endDate)
        {
            //AppUserModel? viewer = await _userManager.FindByIdAsync(viewerId.ToString())
            var allVideos = _videoRepository.GetIQuerableVideos();

            if (beginDate.HasValue)
            {
                allVideos = allVideos.Where(v => v.UploadDate >= beginDate.Value);
            }

            if (endDate.HasValue)
            {
                allVideos = allVideos.Where(v => v.UploadDate <= endDate.Value);
            }

            var videos = allVideos.Where(v => v.Title.Contains(query) || v.Tags.Select(t => t.Content).Contains(query));
            //videos = videos.UnionBy(allVideos.Where(v => v.Tags.Select(t => t.Content).Contains(query)),v =>v.Id);
            //videos = videos.UnionBy(allVideos.Where(v => v.Description.Contains(query)), v => v.Id);
            //videos = videos.UnionBy(allVideos.Where(v => v.Creator.Name.Contains(query) || v.Creator.Surname.Contains(query) || v.Creator.Email.Contains(query) || v.Creator.UserName.Contains(query)), v => v.Id);

            videos = sortingCriteria switch
            {
                SortingTypesEnum.PublishDate => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.UploadDate) : videos.OrderByDescending(v => v.UploadDate),
                SortingTypesEnum.Alphabetical => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.Title) : videos.OrderByDescending(v => v.Title),
                SortingTypesEnum.Popularity => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.ViewCount) : videos.OrderByDescending(v => v.ViewCount),
            };

            var videoList = await videos.Take(10).ToListAsync(); // paginacja by sie przydala

            var playlists = _playlistRepository.GetIQuerablePlaylists();

            playlists = playlists.Where(p => p.Name.Contains(query));// || p.Viewer.Name.Contains(query) || p.Viewer.Surname.Contains(query) || p.Viewer.Email.Contains(query) || p.Viewer.UserName.Contains(query));

            if (sortingCriteria is SortingTypesEnum.Alphabetical)
                playlists = sortingType == SortingDirectionsEnum.Ascending ? playlists.OrderBy(p => p.Name) : playlists.OrderByDescending(p => p.Name);


            var playlistList = await playlists.Take(10).ToListAsync(); // paginacja by sie przydala

            var users = _userManager.Users;

            users = users.Where(u => u.Name.Contains(query) || u.Surname.Contains(query)/* || u.Email.Contains(query)*/ || u.UserName.Contains(query));

            if (sortingCriteria is SortingTypesEnum.Alphabetical)
                users = sortingType == SortingDirectionsEnum.Ascending ? users.OrderBy(u => u.Name) : users.OrderByDescending(u => u.Name);

            var userList = await users.Take(10).ToListAsync(); // paginacja by sie przydala
            var userRoles = await userList.ToAsyncEnumerable().SelectAwait(async u=> (await _userManager.GetRolesAsync(u)).First()).ToListAsync();

            return new SearchResultsDto
            {
                Playlists = playlistList.Select(PlaylistMapper.MapPlaylistModelToPlaylistBaseDto).ToList(),
                Videos = videoList.Select(VideoMapper.VideoModelToVideoMetadataDto).ToList(),
                Users = userList.Zip(userRoles).Select(ur => UserMapper.MapUserModelToUserDto(ur.First, (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), ur.Second))).ToList()
            };
        }
    }
}
