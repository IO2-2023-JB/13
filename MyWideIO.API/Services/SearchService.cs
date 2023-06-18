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
            // videos
            var allVideos = _videoRepository.GetIQuerableVideos();

            allVideos = allVideos.Where(v => v.IsVisible);

            if (beginDate.HasValue)
            {
                allVideos = allVideos.Where(v => v.UploadDate >= beginDate.Value);
            }

            if (endDate.HasValue)
            {
                allVideos = allVideos.Where(v => v.UploadDate <= endDate.Value);
            }

            var videos = allVideos.Where(v => v.Title.Contains(query) || v.Tags.Any(t => t.Content.Contains(query)));
            videos = sortingCriteria switch
            {
                SortingTypesEnum.PublishDate => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.UploadDate) : videos.OrderByDescending(v => v.UploadDate),
                SortingTypesEnum.Alphabetical => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.Title) : videos.OrderByDescending(v => v.Title),
                SortingTypesEnum.Popularity => sortingType == SortingDirectionsEnum.Ascending ? videos.OrderBy(v => v.ViewCount) : videos.OrderByDescending(v => v.ViewCount),
            };

            var videoList = await videos.ToListAsync(); // paginacja by sie przydala

            // playlists

            var playlists = _playlistRepository.GetIQuerablePlaylists();

            playlists = playlists.Where(p => p.IsVisible);

            playlists = playlists.Where(p => p.Name.Contains(query));

            if (sortingCriteria is SortingTypesEnum.Alphabetical)
                playlists = sortingType == SortingDirectionsEnum.Ascending ? playlists.OrderBy(p => p.Name) : playlists.OrderByDescending(p => p.Name);


            var playlistList = await playlists.ToListAsync(); // paginacja by sie przydala

            // users

            var users = _userManager.Users;

            users = users.Where(u => u.Name.Contains(query) || u.Surname.Contains(query)/* || u.Email.Contains(query)*/ || u.UserName.Contains(query));

            if (sortingCriteria is SortingTypesEnum.Alphabetical)
                users = sortingType == SortingDirectionsEnum.Ascending ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName);

            var userList = await users.ToListAsync(); // paginacja by sie przydala
            var userRoles = await userList.ToAsyncEnumerable().SelectAwait(async u => (await _userManager.GetRolesAsync(u)).First()).ToListAsync();

            return new SearchResultsDto
            {
                Playlists = playlistList.Select(p => p.ToPlaylistBaseDto()).ToList(),
                Videos = videoList.Select(v => v.ToVideoMetadataDto()).ToList(),
                Users = userList.Zip(userRoles).Select(ur => ur.First.ToUserDto((UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), ur.Second))).ToList()
            };
        }
    }
}
