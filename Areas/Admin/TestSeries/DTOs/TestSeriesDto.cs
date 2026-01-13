using System.ComponentModel.DataAnnotations;
using RankUpAPI.Models;

namespace RankUpAPI.Areas.Admin.TestSeries.DTOs
{
    public class TestSeriesDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingMarks { get; set; }
        
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTestSeriesDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int DurationInMinutes { get; set; } = 60;
        
        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }
        
        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }

    public class UpdateTestSeriesDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int DurationInMinutes { get; set; } = 60;
        
        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }
        
        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class AddQuestionsToTestSeriesDto
    {
        [Required]
        public int TestSeriesId { get; set; }
        
        [Required]
        public List<int> QuestionIds { get; set; } = new();
    }
}
