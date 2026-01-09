using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    public enum QuestionType
    {
        Text = 1,
        Image = 2,
        Video = 3
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public class Question : BaseEntity
    {
        // Question text in English
        [MaxLength(2000)]
        public string QuestionTextEnglish { get; set; } = string.Empty;

        // Question text in Hindi
        [MaxLength(2000)]
        public string QuestionTextHindi { get; set; } = string.Empty;

        // Question Type
        [Required]
        public QuestionType Type { get; set; } = QuestionType.Text;

        // Question Image (for image-based questions)
        [MaxLength(500)]
        public string? QuestionImageUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? QuestionImageUrlHindi { get; set; }

        // Question Video (for video-based questions)
        [MaxLength(500)]
        public string? QuestionVideoUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? QuestionVideoUrlHindi { get; set; }

        // Options in English
        [MaxLength(500)]
        public string OptionAEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionBEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionCEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionDEnglish { get; set; } = string.Empty;

        // Options in Hindi
        [MaxLength(500)]
        public string OptionAHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionBHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionCHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionDHindi { get; set; } = string.Empty;

        // Option Images (if options have images)
        [MaxLength(500)]
        public string? OptionImageAUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageBUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageCUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageDUrl { get; set; }

        // Correct Answer (A, B, C, or D)
        [Required]
        [MaxLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty; // "A", "B", "C", or "D"

        // Explanation
        [MaxLength(2000)]
        public string? ExplanationEnglish { get; set; }

        [MaxLength(2000)]
        public string? ExplanationHindi { get; set; }

        // Solution Images/Videos
        [MaxLength(500)]
        public string? SolutionImageUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? SolutionImageUrlHindi { get; set; }

        [MaxLength(500)]
        public string? SolutionVideoUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? SolutionVideoUrlHindi { get; set; }

        // Difficulty Level
        [Required]
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;

        // Chapter relationship
        [Required]
        public int ChapterId { get; set; }

        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter { get; set; } = null!;

        // Marks for this question
        public int Marks { get; set; } = 1;

        // Negative marks (if applicable)
        public decimal NegativeMarks { get; set; } = 0;

        // Time to solve (in seconds)
        public int EstimatedTimeInSeconds { get; set; } = 120;

        // Whether this is an MCQ
        public bool IsMcq { get; set; } = true;

        // Navigation properties
        public ICollection<TestSeriesQuestion> TestSeriesQuestions { get; set; } = new List<TestSeriesQuestion>();
    }
}
