using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace AdminService.Infrastructure.Repositories
{
    public class ExportLogDapperRepository : IExportLogRepository
    {
        private readonly IDbConnection _connection;

        public ExportLogDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<ExportLog> AddAsync(ExportLog exportLog)
        {
            var sql = @"EXEC [dbo].[ExportLog_Insert]
                    @AdminId, @ExportType, @FileName, @Format, @Status,
                    @RecordCount, @FilePath, @ErrorMessage,
                    @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", exportLog.AdminId);
            parameters.Add("@ExportType", exportLog.ExportType);
            parameters.Add("@FileName", exportLog.FileName);
            parameters.Add("@Format", exportLog.Format);
            parameters.Add("@Status", exportLog.Status);
            parameters.Add("@RecordCount", exportLog.RecordCount);
            parameters.Add("@FilePath", exportLog.FilePath);
            parameters.Add("@ErrorMessage", exportLog.ErrorMessage ?? (object)DBNull.Value);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                exportLog.Id = parameters.Get<int>("@Id");
            }

            return exportLog;
        }

        public async Task<ExportLog?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[ExportLog_GetById] @Id";
            var parameters = new { Id = id };
            
            var result = await _connection.QueryFirstOrDefaultAsync<ExportLog>(sql, parameters);
            
            return result;
        }

        public async Task<IEnumerable<ExportLog>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50)
        {
            var sql = @"EXEC [dbo].[ExportLog_GetAuditLogs]
                @AdminId, @Page, @PageSize";

            var parameters = new 
            { 
                AdminId = adminId.HasValue ? adminId.Value : (object?)DBNull.Value,
                Page = page,
                PageSize = pageSize
            };
            
            var result = await _connection.QueryAsync<ExportLog>(sql, parameters);
            
            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this method is not needed
            // It's kept for interface compatibility
            return await Task.FromResult(0);
        }
    }
}
