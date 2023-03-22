using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyVideIO.Models;

namespace MyVideIO.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<ViewerModel> Viewers { get; set; }
        public DbSet<CreatorModel> Creators { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<PlaylistModel> Playlists { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        
        ////for testing
        //public ApplicationDbContext()
        //{
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        


    }
}