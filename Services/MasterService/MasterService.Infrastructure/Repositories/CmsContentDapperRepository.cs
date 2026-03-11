using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Infrastructure.Repositories
{
    public class CmsContentDapperRepository : BaseDapperRepository, ICmsContentRepository
    {
        public CmsContentDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<CmsContent?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                    FROM dbo.CmsContents c
                    WHERE c.Id = @Id;
                    
                    SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                    FROM dbo.CmsContentTranslations t
                    WHERE t.CmsContentId = @Id";
                
                using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
                
                var content = await multi.ReadFirstOrDefaultAsync<CmsContent>();
                if (content != null)
                {
                    var translations = await multi.ReadAsync<CmsContentTranslation>();
                    content.Translations = translations.ToList();
                }

                return content;
            });
        }

        public async Task<CmsContent?> GetByKeyAsync(string key)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                    FROM dbo.CmsContents c
                    WHERE c.[Key] = @Key;
                    
                    SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                    FROM dbo.CmsContentTranslations t
                    WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents WHERE [Key] = @Key)";
                
                using var multi = await connection.QueryMultipleAsync(sql, new { Key = key });
                
                var content = await multi.ReadFirstOrDefaultAsync<CmsContent>();
                if (content != null)
                {
                    var translations = await multi.ReadAsync<CmsContentTranslation>();
                    content.Translations = translations.ToList();
                }

                return content;
            });
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[CmsContent_ExistsByKey] @Key";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Key = key });
                return result > 0;
            });
        }

        public async Task<bool> ExistsByKeyExceptIdAsync(string key, int excludeId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[CmsContent_ExistsByKeyExceptId] @Key, @ExcludeId";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Key = key, ExcludeId = excludeId });
                return result > 0;
            });
        }

        public async Task<IEnumerable<CmsContent>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                    FROM dbo.CmsContents c;
                    
                    SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                    FROM dbo.CmsContentTranslations t
                    WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents)";
                
                using var multi = await connection.QueryMultipleAsync(sql);
                
                var contents = (await multi.ReadAsync<CmsContent>()).ToList();
                var translations = (await multi.ReadAsync<CmsContentTranslation>()).ToList();
                
                foreach (var content in contents)
                {
                    content.Translations = translations.Where(t => t.CmsContentId == content.Id).ToList();
                }

                return contents;
            });
        }

        public async Task<IEnumerable<CmsContent>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt
                    FROM dbo.CmsContents c
                    WHERE c.IsActive = 1;
                    
                    SELECT t.Id, t.CmsContentId, t.LanguageCode, t.Title, t.Content
                    FROM dbo.CmsContentTranslations t
                    WHERE t.CmsContentId IN (SELECT Id FROM dbo.CmsContents WHERE IsActive = 1)";
                
                using var multi = await connection.QueryMultipleAsync(sql);
                
                var contents = (await multi.ReadAsync<CmsContent>()).ToList();
                var translations = (await multi.ReadAsync<CmsContentTranslation>()).ToList();
                
                foreach (var content in contents)
                {
                    content.Translations = translations.Where(t => t.CmsContentId == content.Id).ToList();
                }

                return contents;
            });
        }

        public async Task<CmsContent> AddAsync(CmsContent content)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[CmsContent_Create] @Key, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";
                var parameters = new DynamicParameters();
                parameters.Add("@Key", content.Key);
                parameters.Add("@IsActive", content.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
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

                    await connection.ExecuteAsync(tranSql, tranParams);
                    translation.Id = tranParams.Get<int>("@Id");
                }

                return content;
            });
        }

        public async Task UpdateAsync(CmsContent content)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[CmsContent_Update] @Id, @Key, @IsActive, @UpdatedAt";
                var parameters = new DynamicParameters();
                parameters.Add("@Id", content.Id);
                parameters.Add("@Key", content.Key);
                parameters.Add("@IsActive", content.IsActive);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);

                await connection.ExecuteAsync(
                    "EXEC [dbo].[CmsContentTranslation_DeleteByCmsContentId] @CmsContentId",
                    new { CmsContentId = content.Id });

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

                    await connection.ExecuteAsync(tranSql, tranParams);
                }
            });
        }

        public async Task DeleteAsync(CmsContent content)
        {
            await WithConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync(
                    "EXEC [dbo].[CmsContent_Delete] @Id",
                    new { Id = content.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var transaction = connection.BeginTransaction();
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
            });
        }
    }
}