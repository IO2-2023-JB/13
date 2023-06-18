using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Models.DB_Models
{
    public class TicketModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid SubmitterId { get; set; }
        public Guid TargetId { get; set; }
        public string Reason { get; set; }
        public string? Response { get; set; }
        public TicketTargetTypeEnum TargetType { get; set; }
        public TicketStatusEnum Status { get; set; }
    }
}
