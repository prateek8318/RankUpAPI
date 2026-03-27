using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;

namespace AdminService.Infrastructure.Repositories
{
    public class ExportLogDapperRepository : BaseDapperRepository, IExportLogRepository
    {
        public ExportLogDapperRepository(string connectionString, ILogger<ExportLogDapperRepository> logger)
            : base(connectionString, logger)
        {
        }

        public async Task<ExportLog> AddAsync(ExportLog exportLog)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", exportLog.AdminId);
            parameters.Add("@ExportType", exportLog.ExportType);
            parameters.Add("@FileName", exportLog.FileName);
            parameters.Add("@FilePath", exportLog.FilePath);
            parameters.Add("@FileSizeBytes", exportLog.FileSizeBytes);
            parameters.Add("@Format", exportLog.Format);
            parameters.Add("@Status", (int)exportLog.Status);
            parameters.Add("@RecordCount", exportLog.RecordCount);
            parameters.Add("@ErrorMessage", exportLog.ErrorMessage ?? (object)DBNull.Value);
            parameters.Add("@CompletedAt", exportLog.CompletedAt);
            parameters.Add("@FilterCriteria", exportLog.FilterCriteria);
            parameters.Add("@CreatedAt", exportLog.CreatedAt == default ? DateTime.UtcNow : exportLog.CreatedAt);
            parameters.Add("@UpdatedAt", exportLog.UpdatedAt == default ? DateTime.UtcNow : exportLog.UpdatedAt);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(connection =>
                connection.ExecuteAsync(
                    "[dbo].[ExportLog_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
            
            if (parameters.Get<int>("@Id") > 0)
            {
                exportLog.Id = parameters.Get<int>("@Id");
            }

            return exportLog;
        }

        public async Task<ExportLog?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(connection =>
                connection.QueryFirstOrDefaultAsync<ExportLog>(
                    "[dbo].[ExportLog_GetById]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<IEnumerable<ExportLog>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50)
        {
            return await WithConnectionAsync(connection =>
                connection.QueryAsync<ExportLog>(
                    "[dbo].[ExportLog_GetAll]",
                    new
                    {
                        AdminId = adminId,
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
