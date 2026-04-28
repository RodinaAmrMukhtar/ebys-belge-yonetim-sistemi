using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBYS.Models
{
    public class DocumentAttachment
    {
        [Key]
        public int Id { get; set; }

        public int DocumentId { get; set; }
        [ForeignKey(nameof(DocumentId))]
        public Document Document { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
