using System.ComponentModel.DataAnnotations;

namespace TestService.Domain.Entities
{
    public class SubjectMaster : BaseEntity
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ExamMaster Exam { get; set; } = null!;
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    }
}
