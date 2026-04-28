using System.ComponentModel.DataAnnotations;

namespace EBYS.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Optional: Rektörlük / Fakülte / Bölüm
        public string Type { get; set; } = string.Empty;
    }
}
