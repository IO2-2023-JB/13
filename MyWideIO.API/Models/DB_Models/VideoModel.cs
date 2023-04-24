using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using WideIO.API.Models;

namespace MyWideIO.API.Models.DB_Models
{
    public class VideoModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public TimeSpan Duration { get; set; }
        public int ViewCount { get; set; } = 0;
        public int PositiveReactions { get; set; } = 0;

        public int NegativeReactions { get; set; } = 0;

        public ICollection<CommentModel> Comments { get; set; } = new Collection<CommentModel>();
        public ICollection<TagModel> Tags { get; set; } = new Collection<TagModel>();
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public DateTime EditDate { get; set; } = DateTime.Now;

        public Guid CreatorId { get; set; }
        public CreatorModel Creator { get; set; }
        public ICollection<VideoPlaylist> Playlists { get; set; } = new Collection<VideoPlaylist>();
        public ICollection<ViewerLike> LikedBy { get; set; } = new Collection<ViewerLike>();

        // public string fileName { get; set; }
        public ImageModel? Thumbnail { get; set; }
        public bool IsVisible { get; set; }
        public ProcessingProgressEnum ProcessingProgress { get; set; }

    }
}
