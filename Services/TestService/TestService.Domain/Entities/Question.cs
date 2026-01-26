using System.ComponentModel.DataAnnotations;

using TestService.Domain.Enums;

namespace TestService.Domain.Entities
{
    public class Question : BaseEntity
    {
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
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
    }
}
