using System.ComponentModel.DataAnnotations;

namespace TestService.Domain.Entities
{
    public class ExamMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<SubjectMaster> Subjects { get; set; } = new List<SubjectMaster>();
        public virtual ICollection<TestSeries> TestSeries { get; set; } = new List<TestSeries>();
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    }
}
