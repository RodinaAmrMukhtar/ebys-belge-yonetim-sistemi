using System.ComponentModel.DataAnnotations;

namespace EBYS.Models
{
    public class DocumentAudit
    {
        [Key]
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public string Action { get; set; } = string.Empty;

        public string PerformedByUserId { get; set; } = string.Empty;
        public string? PerformedByName { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}
