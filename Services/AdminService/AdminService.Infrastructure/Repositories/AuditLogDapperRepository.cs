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
        
        public AuditLogDapperRepository(AdminDbContext context)
        {
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection is SqlConnection sqlConnection)
                return sqlConnection;
            throw new InvalidOperationException("Database connection is not a SqlConnection");
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[AuditLog_Insert] 
                    @AdminId, @ServiceName, @Action, @EntityType, @EntityId,
                    @OldValues, @NewValues, @IpAddress, @UserAgent, @CreatedAt";

            await connection.ExecuteAsync(sql, auditLog);
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
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

            return await connection.QueryAsync<AuditLog>(sql, parameters);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
