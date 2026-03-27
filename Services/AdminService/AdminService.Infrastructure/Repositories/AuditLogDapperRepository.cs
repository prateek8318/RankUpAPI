using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Repositories
{
    public class AuditLogDapperRepository : BaseDapperRepository, IAuditLogRepository
    {
        public AuditLogDapperRepository(string connectionString, ILogger<AuditLogDapperRepository> logger)
            : base(connectionString, logger)
        {
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            await WithConnectionAsync(connection =>
                connection.ExecuteAsync(
                    "[dbo].[AuditLog_Create]",
                    new
                    {
                        auditLog.AdminId,
                        auditLog.ServiceName,
                        auditLog.Action,
                        auditLog.Endpoint,
                        auditLog.HttpMethod,
                        auditLog.RequestPayload,
                        auditLog.ResponsePayload,
                        auditLog.StatusCode,
                        auditLog.IpAddress,
                        auditLog.UserAgent,
                        auditLog.ResponseTimeMs,
                        auditLog.ErrorMessage,
                        auditLog.CreatedAt
                    },
                    commandType: CommandType.StoredProcedure));
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            return await WithConnectionAsync(connection =>
                connection.QueryAsync<AuditLog>(
                    "[dbo].[AuditLog_GetAuditLogs]",
                    new
                    {
                        AdminId = adminId,
                        ServiceName = serviceName,
                        StartDate = startDate,
                        EndDate = endDate,
                        Page = page,
                        PageSize = pageSize
                    },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this method is not needed
            // It's kept for interface compatibility
            return await Task.FromResult(0);
        }
    }
}
