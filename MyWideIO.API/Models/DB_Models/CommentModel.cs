using System.ComponentModel.DataAnnotations;

namespace MyWideIO.API.Models.DB_Models
{
    public class CommentModel
    {
        public Guid Id { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        public Guid AuthorId { get; set; }
        public AppUserModel Author { get; set; }

        public Guid? VideoId { get; set; }

        public Guid? ParentCommentId { get; set; }

        public bool hasResponses { get; set; }
    }
}