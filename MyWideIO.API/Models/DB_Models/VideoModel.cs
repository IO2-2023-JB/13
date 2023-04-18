using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

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

        public int Duration { get; set; } //in ms?

        public int PositiveReactions { get; set; }

        public int NegativeReactions { get; set; }


        public ICollection<CommentModel> Comments { get; set; }
        public ICollection<string> Tags { get; set; }

        public Guid CreatorId { get; set; }
        public CreatorModel Creator { get; set; }
        public ICollection<VideoPlaylist> Playlists { get; set; }
        public ICollection<ViewerLike> LikedBy { get; set; }

        public string fileName { get; set; }
        public string Thumbnail { get; set; }
        public bool IsVisible { get; set; }


    }
}
