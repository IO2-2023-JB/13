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

        public async Task<PlaylistModel?> GetPlaylistAsync(Guid id)
        {
            return await _dbContext.Playlists
                .Include(p => p.Videos)
                .Where(p=>p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PlaylistModel>> GetUserPlaylists(Guid userId)
        {
            return await _dbContext.Playlists
                .Where(p => p.ViewerId == userId)
                .Include(p => p.Videos)
                .ToListAsync();
        }

        public async Task RemovePlaylistAsync(PlaylistModel playlist)
        {
            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
        }
    }
}
