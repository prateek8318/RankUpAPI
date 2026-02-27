using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class CmsContentDapperRepository : ICmsContentRepository
    {
        private readonly MasterDbContext _context;

        public CmsContentDapperRepository(MasterDbContext context)
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

        public async Task<CmsContent?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<CmsContent>(sql, new { Id = id });
        }

        public async Task<CmsContent?> GetByKeyAsync(string key)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_GetByKey] @Key";
            return await connection.QueryFirstOrDefaultAsync<CmsContent>(sql, new { Key = key });
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_ExistsByKey] @Key";
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Key = key });
            return result > 0;
        }

        public async Task<bool> ExistsByKeyExceptIdAsync(string key, int excludeId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_ExistsByKeyExceptId] @Key, @ExcludeId";
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Key = key, ExcludeId = excludeId });
            return result > 0;
        }

        public async Task<IEnumerable<CmsContent>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_GetAll]";
            return await connection.QueryAsync<CmsContent>(sql);
        }

        public async Task<IEnumerable<CmsContent>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[CmsContent_GetActive]";
            return await connection.QueryAsync<CmsContent>(sql);
        }

        public async Task<CmsContent> AddAsync(CmsContent content)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[CmsContent_Create] 
                    @Key, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Key", content.Key);
            parameters.Add("@IsActive", content.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                content.Id = parameters.Get<int>("@Id");
            }

            return content;
        }

        public Task UpdateAsync(CmsContent content)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = @"
                EXEC [dbo].[CmsContent_Update] 
                    @Id, @Key, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", content.Id);
            parameters.Add("@Key", content.Key);
            parameters.Add("@IsActive", content.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            connection.Execute(sql, parameters);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CmsContent content)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[CmsContent_Delete] @Id";
            connection.Execute(sql, new { Id = content.Id });
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
