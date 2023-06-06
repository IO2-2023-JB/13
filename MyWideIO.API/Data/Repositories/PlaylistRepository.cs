using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Models.DB_Models;

namespace MyWideIO.API.Data.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PlaylistRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Add(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Update(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistModel?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Playlists
                .Include(p => p.VideoPlaylists.OrderBy(vp=>vp.Order))
                .ThenInclude(vp=>vp.Video)
                .ThenInclude(v=>v.Creator)
                .Include(p=>p.Viewer)
                .Where(p=>p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<ICollection<PlaylistModel>> GetUserPlaylistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Playlists
                .Where(p => p.ViewerId == userId)
                .Include(p => p.VideoPlaylists)
                .ThenInclude(vp=>vp.Video)
                .ToListAsync(cancellationToken);
        }

        public async Task RemoveAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<PlaylistModel> GetIQuerablePlaylists()
        {
            return _dbContext.Playlists
                .Include(p=>p.Viewer)
                .AsNoTracking();
        }

        public async Task RemoveAsync(IEnumerable<PlaylistModel> playlists)
        {
            _dbContext.Playlists.RemoveRange(playlists);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlaylistModel>> GetPlaylistsContainingVideo(Guid videoId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Playlists.Where(p => p.VideoPlaylists.Any(vp => vp.VideoId == videoId))
                .Include(p => p.VideoPlaylists)
                .ThenInclude(vp => vp.Video)
                .ToListAsync(cancellationToken);
        }
    }
}
