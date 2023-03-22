using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class VideoModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Please enter the title of your video")]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int Duration { get; set; } //in ms?

        public int PositiveReactions { get; set; }

        public int NegativeReactions { get; set; }

        public IEnumerable<CommentModel> Comments { get; set; }
        

    }
}
