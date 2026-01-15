namespace AdminService.Application.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public long? ResponseTimeMs { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuditLogFilterDto
    {
        public int? AdminId { get; set; }
        public string? ServiceName { get; set; }
        public string? Action { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
