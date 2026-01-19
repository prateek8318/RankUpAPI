using System.ComponentModel.DataAnnotations;

namespace ExamService.Domain.Entities
{
    public class Exam : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        // Navigation property for many-to-many relationship with Qualifications
        public ICollection<ExamQualification> ExamQualifications { get; set; } = new List<ExamQualification>();
    }
}
