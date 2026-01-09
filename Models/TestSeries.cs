using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    public class TestSeries : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int ExamId { get; set; }

        [ForeignKey(nameof(ExamId))]
        public Exam Exam { get; set; } = null!;

        // Test Series specific properties
        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int TotalQuestions { get; set; } = 0;
        public int PassingMarks { get; set; } = 35;

        // Instructions in both languages
        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }

        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }

        // Display order for sorting
        public int DisplayOrder { get; set; } = 0;

        // Whether this test series is locked/unlocked
        public bool IsLocked { get; set; } = false;

        // Navigation properties
        public ICollection<TestSeriesQuestion> TestSeriesQuestions { get; set; } = new List<TestSeriesQuestion>();
    }
}
