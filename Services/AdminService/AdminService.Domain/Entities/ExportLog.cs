using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class ExportLog : BaseEntity
    {
        [Required]
        public int AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ExportType { get; set; } = string.Empty; // e.g., "Users", "Exams", "Subscriptions"

        [MaxLength(500)]
        public string? FilePath { get; set; }

        [MaxLength(200)]
        public string? FileName { get; set; }

        public long? FileSizeBytes { get; set; }

        [MaxLength(50)]
        public string Format { get; set; } = "Excel"; // Excel, CSV, PDF

        public int? RecordCount { get; set; }

        public ExportStatus Status { get; set; } = ExportStatus.Pending;

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(2000)]
        public string? FilterCriteria { get; set; } // JSON string of filters applied
    }

    public enum ExportStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }
}
