using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class Subject : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ExamId { get; set; }

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public virtual Exam Exam { get; set; } = null!;
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
