using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyVideIO.Models;
using MyWideIO.API.Data;

namespace MyVideIO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ViewerModel, UserRole, Guid>
    {

        public DbSet<ViewerModel> Viewers { get; set; }
        public DbSet<CreatorModel> Creators { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<PlaylistModel> Playlists { get; set; }
        public DbSet<CommentModel> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}