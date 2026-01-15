using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class BulkUploadLog : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FilePath { get; set; }

        public int TotalRows { get; set; }

        public int SuccessCount { get; set; }

        public int ErrorCount { get; set; }

        public BulkUploadStatus Status { get; set; } = BulkUploadStatus.Pending;

        [MaxLength(500)]
        public string? ErrorReportPath { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public int ProcessedByUserId { get; set; }

        [MaxLength(2000)]
        public string? ErrorSummary { get; set; }
    }

    public enum BulkUploadStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        PartialSuccess = 5
    }
}
