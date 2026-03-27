using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectDapperRepository : BaseDapperRepository, ISubjectRepository
    {
        public SubjectDapperRepository(string connectionString, ILogger<SubjectDapperRepository> logger) : base(connectionString, logger)
        {
        }

        private const string SubjectLanguageByIdsSql = @"
SELECT
    sl.Id,
    sl.SubjectId,
    sl.LanguageId,
    sl.Name,
    sl.Description,
    sl.IsActive,
    sl.CreatedAt,
    sl.UpdatedAt,
    l.Id,
    l.Name,
    l.Code
FROM dbo.SubjectLanguages sl
LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
WHERE sl.IsActive = 1
  AND sl.SubjectId IN @SubjectIds;";


        public async Task<IEnumerable<Subject>> GetAllAsync(int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = languageId.HasValue 
                    ? "EXEC [dbo].[Subject_GetAllByLanguage] @LanguageId"
                    : "EXEC [dbo].[Subject_GetAll]";
                
                var parameters = new { LanguageId = languageId.HasValue ? (object?)languageId.Value : null };
                var subjectList = (await connection.QueryAsync<Subject>(sql, parameters)).ToList();
                await PopulateSubjectLanguagesAsync(connection, subjectList, languageId);

                return subjectList;
            });
        }

        public async Task<Subject?> GetByIdAsync(int id, int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = languageId.HasValue
                    ? "EXEC [dbo].[Subject_GetByIdByLanguage] @Id, @LanguageId"
                    : "EXEC [dbo].[Subject_GetById] @Id";
                
                var parameters = new { Id = id, LanguageId = languageId.HasValue ? (object?)languageId.Value : null };
                    
                var subject = await connection.QueryFirstOrDefaultAsync<Subject>(sql, parameters);
                
                if (subject != null)
                {
                    await PopulateSubjectLanguagesAsync(connection, new[] { subject }, languageId);
                }
                
                return subject;
            });
        }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync(int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = languageId.HasValue
                    ? "EXEC [dbo].[Subject_GetActiveByLanguage] @LanguageId"
                    : "EXEC [dbo].[Subject_GetActive]";
                
                var parameters = new { LanguageId = languageId.HasValue ? (object?)languageId.Value : null };
                var subjectList = (await connection.QueryAsync<Subject>(sql, parameters)).ToList();
                await PopulateSubjectLanguagesAsync(connection, subjectList, languageId);

                return subjectList;
            });
        }

        public async Task<Subject> AddAsync(Subject subject, string? namesJson = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Subject_Create] 
                        @Name, @Description, @IsActive, @NamesJson, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@NamesJson", namesJson);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    subject.Id = parameters.Get<int>("@Id");
                }

                return subject;
            });
        }

        public async Task<Subject> UpdateAsync(Subject subject, string? namesJson = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Subject_Update] 
                        @Id, @Name, @Description, @IsActive, @NamesJson, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", subject.Id);
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@NamesJson", namesJson);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
                return subject;
            });
        }

        public async Task DeleteAsync(Subject subject)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = subject.Id });
            });
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Subject_ToggleStatus] @Id, @IsActive";
                    var result = await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
                    return result > 0;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error toggling subject status: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_Exists] @Id";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
                return result > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(SubjectDapperRepository));
            return await Task.FromResult(1);
        }

        private async Task PopulateSubjectLanguagesAsync(IDbConnection connection, IEnumerable<Subject> subjects, int? languageId = null)
        {
            var subjectList = subjects.ToList();
            if (subjectList.Count == 0)
            {
                return;
            }

            var subjectIds = subjectList.Select(subject => subject.Id).Distinct().ToArray();
            var sql = languageId.HasValue
                ? @"
                SELECT
                    sl.Id,
                    sl.SubjectId,
                    sl.LanguageId,
                    sl.Name,
                    sl.Description,
                    sl.IsActive,
                    sl.CreatedAt,
                    sl.UpdatedAt,
                    l.Id,
                    l.Name,
                    l.Code
                FROM dbo.SubjectLanguages sl
                LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
                WHERE sl.IsActive = 1
                  AND sl.SubjectId IN @SubjectIds
                  AND sl.LanguageId = @LanguageId;"
                : @"
                SELECT
                    sl.Id,
                    sl.SubjectId,
                    sl.LanguageId,
                    sl.Name,
                    sl.Description,
                    sl.IsActive,
                    sl.CreatedAt,
                    sl.UpdatedAt,
                    l.Id,
                    l.Name,
                    l.Code
                FROM dbo.SubjectLanguages sl
                LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
                WHERE sl.IsActive = 1
                  AND sl.SubjectId IN @SubjectIds;";
                  
            var parameters = new { SubjectIds = subjectIds, LanguageId = languageId.HasValue ? (object?)languageId.Value : null };
                
            var languages = await connection.QueryAsync<SubjectLanguage>(sql, parameters);
            RepositoryEntityMapper.AttachSubjectLanguages(subjectList, languages);
        }
    }
}
