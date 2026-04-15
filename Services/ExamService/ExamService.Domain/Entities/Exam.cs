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
        
        public bool IsInternational { get; set; } = false; // Default to Indian exams

        // New mapped properties
        public int? ExamCategoryId { get; set; }
        public int? ExamTypeId { get; set; }
        public int? SubjectId { get; set; }

        public int TotalQuestions { get; set; }
        public decimal MarksPerQuestion { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }

        [MaxLength(20)]
        public string AccessType { get; set; } = "Free"; // Free, Paid
        
        public int? SubscriptionPlanId { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? ValidTill { get; set; }

        public int AttemptsAllowed { get; set; } = 1;

        [MaxLength(50)]
        public string ShowResultType { get; set; } = "Immediately"; 

        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Active
        
        // Navigation properties
        public ExamCategory? ExamCategory { get; set; }
        public ExamType? ExamType { get; set; }
        public ICollection<ExamQualification> ExamQualifications { get; set; } = new List<ExamQualification>();
    }
}
