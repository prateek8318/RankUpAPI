using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;

namespace AdminService.Infrastructure.Repositories
{
    public class AuditLogDapperRepository : IAuditLogRepository
    {
        private readonly AdminDbContext _context;
        private readonly IDbConnection _connection;

        public AuditLogDapperRepository(AdminDbContext context, IDbConnection connection)
        {
            _context = context;
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
            return await _context.SaveChangesAsync();
        }
    }
}
