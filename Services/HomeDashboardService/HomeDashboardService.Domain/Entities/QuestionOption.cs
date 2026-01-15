using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class QuestionOption : BaseEntity
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string OptionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsCorrect { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public virtual Question Question { get; set; } = null!;
    }
}
