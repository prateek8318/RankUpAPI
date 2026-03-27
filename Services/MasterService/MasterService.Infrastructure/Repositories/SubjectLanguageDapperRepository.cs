using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectLanguageDapperRepository : BaseDapperRepository, ISubjectLanguageRepository
    {
        public SubjectLanguageDapperRepository(string connectionString, ILogger<SubjectLanguageDapperRepository> logger) : base(connectionString, logger)
        {
        }

        public async Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
                return await connection.QueryAsync<SubjectLanguage>(sql, new { SubjectId = subjectId });
            });
        }

        public async Task<SubjectLanguage?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubjectLanguage_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<SubjectLanguage>(sql, new { Id = id });
            });
        }

        public async Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                EXEC [dbo].[SubjectLanguage_Create] 
                    @SubjectId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@SubjectId", subjectLanguage.SubjectId);
                parameters.Add("@LanguageId", subjectLanguage.LanguageId);
                parameters.Add("@Name", subjectLanguage.Name);
                parameters.Add("@Description", subjectLanguage.Description);
                parameters.Add("@IsActive", subjectLanguage.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    subjectLanguage.Id = parameters.Get<int>("@Id");
                }

                return subjectLanguage;
            });
        }

        public async Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                EXEC [dbo].[SubjectLanguage_Update] 
                    @Id, @SubjectId, @LanguageId, @Name, @Description, @IsActive, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", subjectLanguage.Id);
                parameters.Add("@SubjectId", subjectLanguage.SubjectId);
                parameters.Add("@LanguageId", subjectLanguage.LanguageId);
                parameters.Add("@Name", subjectLanguage.Name);
                parameters.Add("@Description", subjectLanguage.Description);
                parameters.Add("@IsActive", subjectLanguage.IsActive);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
                return subjectLanguage;
            });
        }
        public async Task DeleteAsync(SubjectLanguage subjectLanguage)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubjectLanguage_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = subjectLanguage.Id });
            });
        }
        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(SubjectLanguageDapperRepository));
            return await Task.FromResult(1);
        }
    }
}
