using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    public class Qualification : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation property for many-to-many relationship
        public ICollection<ExamQualification> ExamQualifications { get; set; } = new List<ExamQualification>();
    }
}
