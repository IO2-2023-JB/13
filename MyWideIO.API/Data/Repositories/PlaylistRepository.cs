using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class PlaylistRepository : Repository<PlaylistModel>, IPlaylistRepository
    {

        public PlaylistRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PlaylistModel?> GetAsync(Guid id, bool includeVideos, CancellationToken cancellationToken)
        {
            IQueryable<PlaylistModel> query = _dbContext.Playlists;
            if (includeVideos)
                query = query.Include(p => p.VideoPlaylists.OrderBy(vp => vp.Order))
               .ThenInclude(vp => vp.Video)
               .ThenInclude(v => v.Creator)
               .Include(p => p.VideoPlaylists)
               .ThenInclude(vp => vp.Video)
               .ThenInclude(v => v.Tags);
            return await query
                  .Include(p => p.Viewer)
                  .Where(p => p.Id == id)
                  .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<ICollection<PlaylistModel>> GetUserPlaylistsAsync(Guid userId, bool includeVideos, CancellationToken cancellationToken)
        {
            IQueryable<PlaylistModel> query = _dbContext.Playlists;
            if (includeVideos)
                query = query
                    .Include(p => p.VideoPlaylists.OrderBy(vp => vp.Order))
                    .ThenInclude(vp => vp.Video)
                    .ThenInclude(v => v.Creator)
                    .Include(p => p.VideoPlaylists)
                    .ThenInclude(vp => vp.Video)
                    .ThenInclude(v => v.Tags);
            return await query
                .Where(p => p.ViewerId == userId)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<PlaylistModel> GetIQuerablePlaylists()
        {
            return _dbContext.Playlists
                .Include(p => p.Viewer)
                .AsNoTracking();
        }

        public async Task<List<PlaylistModel>> GetPlaylistsContainingVideo(Guid videoId, bool includeVideos, CancellationToken cancellationToken = default)
        {
            IQueryable<PlaylistModel> query = _dbContext.Playlists;
            if (includeVideos)
                query = query
                    .Include(p => p.VideoPlaylists.OrderBy(vp => vp.Order))
                    .ThenInclude(vp => vp.Video)
                    .ThenInclude(v => v.Creator)
                    .Include(p => p.VideoPlaylists)
                    .ThenInclude(vp => vp.Video)
                    .ThenInclude(v => v.Tags);

            return await query
                .Where(p => p.VideoPlaylists.Any(vp => vp.VideoId == videoId))
                .ToListAsync(cancellationToken);
        }


    }
}
