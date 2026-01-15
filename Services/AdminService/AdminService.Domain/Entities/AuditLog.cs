using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        [Required]
        public int AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // e.g., "CREATE", "UPDATE", "DELETE"

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; } = string.Empty; // e.g., "ExamService", "UserService"

        [Required]
        [MaxLength(200)]
        public string Endpoint { get; set; } = string.Empty; // e.g., "/api/exams/123"

        [MaxLength(50)]
        public string HttpMethod { get; set; } = string.Empty; // GET, POST, PUT, DELETE

        [MaxLength(2000)]
        public string? RequestPayload { get; set; } // JSON string

        [MaxLength(2000)]
        public string? ResponsePayload { get; set; } // JSON string

        public int? StatusCode { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public long? ResponseTimeMs { get; set; }

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }
    }
}
