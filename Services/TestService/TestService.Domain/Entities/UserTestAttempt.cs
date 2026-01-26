using System.ComponentModel.DataAnnotations;

using TestService.Domain.Enums;

namespace TestService.Domain.Entities
{
    public class UserTestAttempt : BaseEntity
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CurrentQuestionIndex { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int TotalMarks { get; set; } = 0;
        public decimal Accuracy { get; set; } = 0;
        public TestAttemptStatus Status { get; set; } = TestAttemptStatus.NotStarted;

        [MaxLength(1000)]
        public string? AnswersJson { get; set; }

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
    }
}
