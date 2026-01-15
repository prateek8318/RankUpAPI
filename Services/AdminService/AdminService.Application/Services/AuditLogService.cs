using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AdminService.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            ILogger<AuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task LogAuditAsync(int adminId, string action, string serviceName, string endpoint, string httpMethod, string? requestPayload = null, string? responsePayload = null, int? statusCode = null, string? ipAddress = null, string? userAgent = null, long? responseTimeMs = null, string? errorMessage = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    AdminId = adminId,
                    Action = action,
                    ServiceName = serviceName,
                    Endpoint = endpoint,
                    HttpMethod = httpMethod,
                    RequestPayload = requestPayload,
                    ResponsePayload = responsePayload,
                    StatusCode = statusCode,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    ResponseTimeMs = responseTimeMs,
                    ErrorMessage = errorMessage,
                    CreatedAt = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit entry");
                // Don't throw - audit logging should not break the main flow
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var logs = await _auditLogRepository.GetAuditLogsAsync(adminId, serviceName, startDate, endDate, page, pageSize);
                
                return logs.Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    AdminId = log.AdminId,
                    AdminEmail = $"Admin_{log.AdminId}", // Admin references UserId, would need UserService call to get email
                    Action = log.Action,
                    ServiceName = log.ServiceName,
                    Endpoint = log.Endpoint,
                    HttpMethod = log.HttpMethod,
                    RequestPayload = log.RequestPayload,
                    ResponsePayload = log.ResponsePayload,
                    StatusCode = log.StatusCode,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    ResponseTimeMs = log.ResponseTimeMs,
                    ErrorMessage = log.ErrorMessage,
                    CreatedAt = log.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs");
                throw;
            }
        }
    }
}
