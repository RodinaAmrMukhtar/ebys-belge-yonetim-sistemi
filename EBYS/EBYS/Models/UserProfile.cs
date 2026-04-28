using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EBYS.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        // Identity link
        public string IdentityUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(IdentityUserId))]
        public IdentityUser IdentityUser { get; set; } = null!;

        // EBYS visible info
        public string FullName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        // Department
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;
    }
}
