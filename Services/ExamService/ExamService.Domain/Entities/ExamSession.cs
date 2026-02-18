using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Language;

namespace ExamService.Domain.Entities
{
    public class ExamSession : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        [MaxLength(5)]
        [LanguageValidation]
        public string ExamLanguage { get; set; } = LanguageConstants.DefaultLanguage;
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndTime { get; set; }
        
        [Required]
        public int DurationInMinutes { get; set; }
        
        public DateTime? LastPauseTime { get; set; }
        
        public int TotalPauseDuration { get; set; } = 0;
        
        public bool IsCompleted { get; set; } = false;
        
        public bool IsPaused { get; set; } = false;
        
        public int CurrentQuestionIndex { get; set; } = 0;
        
        public int Score { get; set; } = 0;
        
        public int CorrectAnswers { get; set; } = 0;
        
        public int IncorrectAnswers { get; set; } = 0;
        
        public int SkippedQuestions { get; set; } = 0;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Navigation properties
        // Note: User and Exam entities should be handled via service calls or separate queries
        public virtual ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
    }
}
