using System.Text.Json;
using AdminService.Domain.Entities;

namespace AdminService.Application.DTOs
{
    public class ExportResultDto
    {
        public int ExportLogId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public int RecordCount { get; set; }
        public ExportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExportFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string? SearchTerm { get; set; }
        public Dictionary<string, object>? AdditionalFilters { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ExportLogDto
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string ExportType { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string Format { get; set; } = string.Empty;
        public int? RecordCount { get; set; }
        public ExportStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
