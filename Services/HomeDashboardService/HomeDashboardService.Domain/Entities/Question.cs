using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class Question : BaseEntity
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(2000)]
        public string? Explanation { get; set; }

        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;

        public int Marks { get; set; } = 1;

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }

    public enum QuestionDifficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }
}
