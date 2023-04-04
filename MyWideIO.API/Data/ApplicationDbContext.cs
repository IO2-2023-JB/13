using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWideIO.API.Models.DB_Models;
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


            modelBuilder.Entity<ViewerWatchLater>()
                .HasKey(vw => new { vw.ViewerId, vw.VideoId });
            
            modelBuilder.Entity<ViewerWatchLater>()
                .HasOne(vw => vw.Viewer)
                .WithMany(v => v.WatchLater)
                .HasForeignKey(vw => vw.ViewerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ViewerWatchLater>()
                .HasOne(vw => vw.Video)
                .WithMany()
                .HasForeignKey(vw => vw.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ViewerSubscription>()
                .HasKey(vs => new { vs.ViewerId, vs.CreatorId });

            modelBuilder.Entity<ViewerSubscription>()
                .HasOne(vs => vs.Viewer)
                .WithMany(v => v.Subscriptions)
                .HasForeignKey(vs => vs.ViewerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ViewerSubscription>()
                .HasOne(vs => vs.Creator)
                .WithMany(c => c.Subscribers)
                .HasForeignKey(vs => vs.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ViewerLike>()
                .HasKey(vl => new { vl.ViewerId, vl.VideoId });

            modelBuilder.Entity<ViewerLike>()
                .HasOne(vl => vl.Viewer)
                .WithMany(v => v.Likes)
                .HasForeignKey(vl => vl.ViewerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ViewerLike>()
                .HasOne(vl => vl.Video)
                .WithMany(v => v.LikedBy)
                .HasForeignKey(vl => vl.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VideoPlaylist>()
                .HasKey(vp => new { vp.VideoId, vp.PlaylistId });

            modelBuilder.Entity<VideoPlaylist>()
                .HasOne(vp => vp.Video)
                .WithMany(v => v.Playlists)
                .HasForeignKey(vp => vp.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VideoPlaylist>()
                .HasOne(vp => vp.Playlist)
                .WithMany(p => p.VideoPlaylists)
                .HasForeignKey(vp => vp.PlaylistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}