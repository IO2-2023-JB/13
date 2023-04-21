using System.ComponentModel.DataAnnotations;

namespace MyWideIO.API.Models.DB_Models
{
    public class TagModel
    {
        public Guid Id { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        public VideoModel Parent { get; set; }
    }
}
