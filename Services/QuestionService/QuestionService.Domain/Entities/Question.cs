using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
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
        [MaxLength(2000)]
        public string QuestionTextEnglish { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string QuestionTextHindi { get; set; } = string.Empty;

        [Required]
        public QuestionType Type { get; set; } = QuestionType.Text;

        [MaxLength(500)]
        public string? QuestionImageUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? QuestionImageUrlHindi { get; set; }

        [MaxLength(500)]
        public string? QuestionVideoUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? QuestionVideoUrlHindi { get; set; }

        [MaxLength(500)]
        public string OptionAEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionBEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionCEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionDEnglish { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionAHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionBHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionCHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OptionDHindi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? OptionImageAUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageBUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageCUrl { get; set; }

        [MaxLength(500)]
        public string? OptionImageDUrl { get; set; }

        [Required]
        [MaxLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? ExplanationEnglish { get; set; }

        [MaxLength(2000)]
        public string? ExplanationHindi { get; set; }

        [MaxLength(500)]
        public string? SolutionImageUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? SolutionImageUrlHindi { get; set; }

        [MaxLength(500)]
        public string? SolutionVideoUrlEnglish { get; set; }

        [MaxLength(500)]
        public string? SolutionVideoUrlHindi { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;

        public int ChapterId { get; set; } // Reference to QuizService

        public int Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public int EstimatedTimeInSeconds { get; set; } = 120;
        public bool IsMcq { get; set; } = true;
    }
}
