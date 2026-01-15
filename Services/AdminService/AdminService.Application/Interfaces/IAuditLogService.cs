using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAuditAsync(int adminId, string action, string serviceName, string endpoint, string httpMethod, string? requestPayload = null, string? responsePayload = null, int? statusCode = null, string? ipAddress = null, string? userAgent = null, long? responseTimeMs = null, string? errorMessage = null);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50);
    }
}
