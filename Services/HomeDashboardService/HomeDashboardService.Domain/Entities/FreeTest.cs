using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class FreeTest : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        public int QuizId { get; set; }

        public int ExamId { get; set; }

        public int DurationMinutes { get; set; } = 60;

        public int TotalQuestions { get; set; } = 0;

        public int TotalMarks { get; set; } = 100;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;

        [MaxLength(500)]
        public string? LinkUrl { get; set; }
    }
}
