using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Repositories
{
    public class AuditLogDapperRepository : IAuditLogRepository
    {
        private readonly IDbConnection _connection;

        public AuditLogDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            var sql = @"
                EXEC [dbo].[AuditLog_Insert] 
                    @AdminId, @ServiceName, @Action, @EntityType, @EntityId,
                    @OldValues, @NewValues, @IpAddress, @UserAgent, @CreatedAt";

            await _connection.ExecuteAsync(sql, auditLog);
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            var sql = "EXEC [dbo].[AuditLog_GetAuditLogs] @AdminId, @ServiceName, @StartDate, @EndDate, @Page, @PageSize";
            
            var parameters = new
            {
                AdminId = adminId,
                ServiceName = serviceName,
                StartDate = startDate,
                EndDate = endDate,
                Page = page,
                PageSize = pageSize
            };

            return await _connection.QueryAsync<AuditLog>(sql, parameters);
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this method is not needed
            // It's kept for interface compatibility
            return await Task.FromResult(0);
        }
    }
}
