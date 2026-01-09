using System.ComponentModel.DataAnnotations;
using RankUpAPI.Models;

namespace RankUpAPI.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        
        public string QuestionTextEnglish { get; set; } = string.Empty;
        public string QuestionTextHindi { get; set; } = string.Empty;
        
        public QuestionType Type { get; set; }
        
        public string? QuestionImageUrlEnglish { get; set; }
        public string? QuestionImageUrlHindi { get; set; }
        public string? QuestionVideoUrlEnglish { get; set; }
        public string? QuestionVideoUrlHindi { get; set; }
        
        public string OptionAEnglish { get; set; } = string.Empty;
        public string OptionBEnglish { get; set; } = string.Empty;
        public string OptionCEnglish { get; set; } = string.Empty;
        public string OptionDEnglish { get; set; } = string.Empty;
        
        public string OptionAHindi { get; set; } = string.Empty;
        public string OptionBHindi { get; set; } = string.Empty;
        public string OptionCHindi { get; set; } = string.Empty;
        public string OptionDHindi { get; set; } = string.Empty;
        
        public string? OptionImageAUrl { get; set; }
        public string? OptionImageBUrl { get; set; }
        public string? OptionImageCUrl { get; set; }
        public string? OptionImageDUrl { get; set; }
        
        [Required]
        [MaxLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? ExplanationEnglish { get; set; }
        public string? ExplanationHindi { get; set; }
        
        public string? SolutionImageUrlEnglish { get; set; }
        public string? SolutionImageUrlHindi { get; set; }
        public string? SolutionVideoUrlEnglish { get; set; }
        public string? SolutionVideoUrlHindi { get; set; }
        
        public DifficultyLevel Difficulty { get; set; }
        
        public int ChapterId { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        
        public int Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public int EstimatedTimeInSeconds { get; set; }
        public bool IsMcq { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateQuestionDto
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
        
        [Required]
        [MaxLength(500)]
        public string OptionAEnglish { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string OptionBEnglish { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string OptionCEnglish { get; set; } = string.Empty;
        
        [Required]
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
        [RegularExpression("^[A-D]$", ErrorMessage = "CorrectAnswer must be A, B, C, or D")]
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
        
        [Required]
        public int ChapterId { get; set; }
        
        public int Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public int EstimatedTimeInSeconds { get; set; } = 120;
        public bool IsMcq { get; set; } = true;
    }

    public class UpdateQuestionDto
    {
        public int Id { get; set; }
        
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
        
        [Required]
        [MaxLength(500)]
        public string OptionAEnglish { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string OptionBEnglish { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string OptionCEnglish { get; set; } = string.Empty;
        
        [Required]
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
        [RegularExpression("^[A-D]$", ErrorMessage = "CorrectAnswer must be A, B, C, or D")]
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
        
        [Required]
        public int ChapterId { get; set; }
        
        public int Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public int EstimatedTimeInSeconds { get; set; } = 120;
        public bool IsMcq { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }

    public class BulkUploadQuestionDto
    {
        public List<CreateQuestionDto> Questions { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
