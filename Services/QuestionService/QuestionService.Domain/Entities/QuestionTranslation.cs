using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
{
    public class QuestionTranslation : BaseEntity
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = "en"; // en, hi, pa, ta, te

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? OptionA { get; set; }

        [MaxLength(500)]
        public string? OptionB { get; set; }

        [MaxLength(500)]
        public string? OptionC { get; set; }

        [MaxLength(500)]
        public string? OptionD { get; set; }

        public string? Explanation { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; } = null!;
    }
}
