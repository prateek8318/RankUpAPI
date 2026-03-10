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
        private SqlTransaction? _currentTransaction;

        public CmsContentDapperRepository(MasterDbContext context)
        {
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            var connectionString = _context.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not initialized in MasterDbContext");
                
            return new SqlConnection(connectionString);
        }

        private async Task EnsureOpenAsync(SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
        }

        public async Task<CmsContent?> GetByIdAsync(int id)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"
                SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                FROM dbo.CmsContents c
                WHERE c.Id = @Id;
                
                SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                FROM dbo.CmsContentTranslations t
                WHERE t.CmsContentId = @Id";
            
            using var multi = await connection.QueryMultipleAsync(sql, new { Id = id }, transaction: _currentTransaction);
            
            var content = await multi.ReadFirstOrDefaultAsync<CmsContent>();
            if (content != null)
            {
                var translations = await multi.ReadAsync<CmsContentTranslation>();
                content.Translations = translations.ToList();
            }

            return content;
        }

        public async Task<CmsContent?> GetByKeyAsync(string key)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"
                SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                FROM dbo.CmsContents c
                WHERE c.[Key] = @Key;
                
                SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                FROM dbo.CmsContentTranslations t
                WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents WHERE [Key] = @Key)";
            
            using var multi = await connection.QueryMultipleAsync(sql, new { Key = key }, transaction: _currentTransaction);
            
            var content = await multi.ReadFirstOrDefaultAsync<CmsContent>();
            if (content != null)
            {
                var translations = await multi.ReadAsync<CmsContentTranslation>();
                content.Translations = translations.ToList();
            }

            return content;
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = "EXEC [dbo].[CmsContent_ExistsByKey] @Key";
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                sql,
                new { Key = key },
                transaction: _currentTransaction
            );
            return result > 0;
        }

        public async Task<bool> ExistsByKeyExceptIdAsync(string key, int excludeId)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = "EXEC [dbo].[CmsContent_ExistsByKeyExceptId] @Key, @ExcludeId";
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                sql,
                new { Key = key, ExcludeId = excludeId },
                transaction: _currentTransaction
            );
            return result > 0;
        }

        public async Task<IEnumerable<CmsContent>> GetAllAsync()
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"
                SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                FROM dbo.CmsContents c;
                
                SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                FROM dbo.CmsContentTranslations t
                WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents)";
            
            using var multi = await connection.QueryMultipleAsync(sql, transaction: _currentTransaction);
            
            var contents = (await multi.ReadAsync<CmsContent>()).ToList();
            var translations = (await multi.ReadAsync<CmsContentTranslation>()).ToList();
            
            foreach (var content in contents)
            {
                content.Translations = translations.Where(t => t.CmsContentId == content.Id).ToList();
            }

            return contents;
        }

        public async Task<IEnumerable<CmsContent>> GetActiveAsync()
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"
                SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                FROM dbo.CmsContents c
                WHERE c.IsActive = 1;
                
                SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                FROM dbo.CmsContentTranslations t
                WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents WHERE IsActive = 1);";
            
            using var multi = await connection.QueryMultipleAsync(sql, transaction: _currentTransaction);
            
            var contents = (await multi.ReadAsync<CmsContent>()).ToList();
            var translations = (await multi.ReadAsync<CmsContentTranslation>()).ToList();
            
            foreach (var content in contents)
            {
                content.Translations = translations.Where(t => t.CmsContentId == content.Id).ToList();
            }

            return contents;
        }

        public async Task<CmsContent> AddAsync(CmsContent content)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"EXEC [dbo].[CmsContent_Create] @Key, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";
            var parameters = new DynamicParameters();
            parameters.Add("@Key", content.Key);
            parameters.Add("@IsActive", content.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters, transaction: _currentTransaction);
            content.Id = parameters.Get<int>("@Id");

            foreach (var translation in content.Translations)
            {
                translation.CmsContentId = content.Id;
                var tranSql = @"EXEC [dbo].[CmsContentTranslation_Create] 
                    @CmsContentId, @LanguageCode, @Title, @Content, @Id OUTPUT";

                var tranParams = new DynamicParameters();
                tranParams.Add("@CmsContentId", translation.CmsContentId);
                tranParams.Add("@LanguageCode", translation.LanguageCode);
                tranParams.Add("@Title", translation.Title);
                tranParams.Add("@Content", translation.Content);
                tranParams.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(tranSql, tranParams, transaction: _currentTransaction);
                translation.Id = tranParams.Get<int>("@Id");
            }

            return content;
        }

        public async Task UpdateAsync(CmsContent content)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            var sql = @"EXEC [dbo].[CmsContent_Update] @Id, @Key, @IsActive, @UpdatedAt";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", content.Id);
            parameters.Add("@Key", content.Key);
            parameters.Add("@IsActive", content.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await connection.ExecuteAsync(sql, parameters, transaction: _currentTransaction);

            await connection.ExecuteAsync(
                "EXEC [dbo].[CmsContentTranslation_DeleteByCmsContentId] @CmsContentId",
                new { CmsContentId = content.Id },
                transaction: _currentTransaction
            );

            foreach (var translation in content.Translations)
            {
                translation.CmsContentId = content.Id;
                var tranSql = @"EXEC [dbo].[CmsContentTranslation_Create] 
                    @CmsContentId, @LanguageCode, @Title, @Content, @Id OUTPUT";

                var tranParams = new DynamicParameters();
                tranParams.Add("@CmsContentId", translation.CmsContentId);
                tranParams.Add("@LanguageCode", translation.LanguageCode);
                tranParams.Add("@Title", translation.Title);
                tranParams.Add("@Content", translation.Content);
                tranParams.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(tranSql, tranParams, transaction: _currentTransaction);
            }
        }

        public async Task DeleteAsync(CmsContent content)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            await connection.ExecuteAsync(
                "EXEC [dbo].[CmsContent_Delete] @Id",
                new { Id = content.Id },
                transaction: _currentTransaction
            );
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(1);
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            var connection = GetConnection();
            await EnsureOpenAsync(connection);

            using var transaction = connection.BeginTransaction();
            _currentTransaction = transaction;
            try
            {
                var result = await operation();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _currentTransaction = null;
            }
        }
    }
}