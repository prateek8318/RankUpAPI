using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectLanguageDapperRepository : ISubjectLanguageRepository
    {
        private readonly MasterDbContext _context;

        public SubjectLanguageDapperRepository(MasterDbContext context)
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

        public async Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
            return await connection.QueryAsync<SubjectLanguage>(sql, new { SubjectId = subjectId });
        }

        public async Task<SubjectLanguage> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubjectLanguage_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<SubjectLanguage>(sql, new { Id = id }) ?? new SubjectLanguage();
        }

        public async Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
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

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                subjectLanguage.Id = parameters.Get<int>("@Id");
            }

            return subjectLanguage;
        }

        public Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage)
        {
            using var connection = GetConnection();
            connection.Open();
            
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

            connection.Execute(sql, parameters);
            return Task.FromResult(subjectLanguage);
        }

        public Task DeleteAsync(SubjectLanguage subjectLanguage)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[SubjectLanguage_Delete] @Id";
            connection.Execute(sql, new { Id = subjectLanguage.Id });
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
