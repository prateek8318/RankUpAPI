using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
{
    public enum BatchStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }

    public class QuestionBatch : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string BatchName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int ProcessedQuestions { get; set; } = 0;

        [Required]
        public int FailedQuestions { get; set; } = 0;

        [Required]
        public BatchStatus Status { get; set; } = BatchStatus.Pending;

        public string? ErrorMessage { get; set; }

        [Required]
        public int UploadedBy { get; set; } // Admin user ID

        // Navigation properties
        public virtual ICollection<QuestionError> Errors { get; set; } = new List<QuestionError>();
    }
}
