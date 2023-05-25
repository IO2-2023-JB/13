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

        public async Task AddPlaylistAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Add(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePlaylistAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Update(playlist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistModel?> GetPlaylistAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Playlists
                .Include(p => p.VideoPlaylists.OrderBy(vp=>vp.Order))
                .ThenInclude(vp=>vp.Video)
                .ThenInclude(v=>v.Creator)
                .Include(p=>p.Viewer)
                .Where(p=>p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ICollection<PlaylistModel>> GetUserPlaylists(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Playlists
                .Where(p => p.ViewerId == userId)
                .Include(p => p.VideoPlaylists)
                .ThenInclude(vp=>vp.Video)
                .ToListAsync(cancellationToken);
        }

        public async Task RemovePlaylistAsync(PlaylistModel playlist)
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
    }
}
