using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Domain.Entities
{
    public class ExamAnswer : BaseEntity
    {
        [Required]
        public int ExamSessionId { get; set; }
        
        [Required]
        public int QuestionId { get; set; }
        
        public int? SelectedOptionId { get; set; }
        
        [MaxLength(2000)]
        public string? TextAnswer { get; set; }
        
        public bool IsCorrect { get; set; } = false;
        
        public int PointsEarned { get; set; } = 0;
        
        public int TimeSpentSeconds { get; set; } = 0;
        
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
        
        public bool IsMarkedForReview { get; set; } = false;
        
        public bool IsSkipped { get; set; } = false;
        
        // Navigation properties
        [ForeignKey("ExamSessionId")]
        public virtual ExamSession ExamSession { get; set; } = null!;
        
        // Note: Question and QuestionOption entities should be handled via service calls or separate queries
    }
}
