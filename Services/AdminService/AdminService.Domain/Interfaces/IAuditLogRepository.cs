using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50);
        Task<int> SaveChangesAsync();
    }
}
