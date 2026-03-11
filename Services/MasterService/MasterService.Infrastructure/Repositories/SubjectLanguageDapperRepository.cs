using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectLanguageDapperRepository : BaseDapperRepository, ISubjectLanguageRepository
    {
        public SubjectLanguageDapperRepository(string connectionString) : base(connectionString)
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

        public async Task<SubjectLanguage> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[SubjectLanguage_GetById] @Id";
            return await _connection.QueryFirstOrDefaultAsync<SubjectLanguage>(sql, new { Id = id }) ?? new SubjectLanguage();
        }

        public async Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage)
        {
            var sql = @"
                EXEC [dbo].[SubjectLanguage_Create] 
                    @SubjectId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@SubjectId", subjectLanguage.SubjectId);
            parameters.Add("@LanguageId", subjectLanguage.LanguageId);
            parameters.Add("@Name", subjectLanguage.Name);
            parameters.Add("@IsActive", subjectLanguage.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                subjectLanguage.Id = parameters.Get<int>("@Id");
            }

            return subjectLanguage;
        }

        public async Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage)
        {
            var sql = @"
                EXEC [dbo].[SubjectLanguage_Update] 
                    @Id, @SubjectId, @LanguageId, @Name, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", subjectLanguage.Id);
            parameters.Add("@SubjectId", subjectLanguage.SubjectId);
            parameters.Add("@LanguageId", subjectLanguage.LanguageId);
            parameters.Add("@Name", subjectLanguage.Name);
            parameters.Add("@IsActive", subjectLanguage.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await _connection.ExecuteAsync(sql, parameters);
            return subjectLanguage;
        }
        public async Task DeleteAsync(SubjectLanguage subjectLanguage)
        {
            var sql = "EXEC [dbo].[SubjectLanguage_Delete] @Id";
            await _connection.ExecuteAsync(sql, new { Id = subjectLanguage.Id });
        }
        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(1);
        }
    }
}
