using System.ComponentModel.DataAnnotations;

namespace QuizService.Domain.Entities
{
    public class TestSeries : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int ExamId { get; set; } // Reference to ExamService

        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int TotalQuestions { get; set; } = 0;
        public int PassingMarks { get; set; } = 35;

        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }

        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }
}
