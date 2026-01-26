using System.ComponentModel.DataAnnotations;

namespace TestService.Domain.Entities
{
    public class Test : BaseEntity
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public int PracticeModeId { get; set; }

        // Optional filters based on practice mode
        public int? SeriesId { get; set; } // NULL for Mock Test, Deep Practice, Previous Year
        public int? SubjectId { get; set; } // NULL for Mock Test, Test Series
        public int? Year { get; set; } // NULL except for Previous Year

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DurationInMinutes { get; set; } = 60;
        public int TotalQuestions { get; set; } = 0;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;

        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }

        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;

        // Navigation properties
        public virtual ExamMaster Exam { get; set; } = null!;
        public virtual PracticeMode PracticeMode { get; set; } = null!;
        public virtual TestSeries? Series { get; set; }
        public virtual SubjectMaster? Subject { get; set; }
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public virtual ICollection<UserTestAttempt> UserTestAttempts { get; set; } = new List<UserTestAttempt>();
    }
}
