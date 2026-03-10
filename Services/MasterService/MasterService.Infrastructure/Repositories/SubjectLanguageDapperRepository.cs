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
        private readonly IDbConnection _connection;

        public SubjectLanguageDapperRepository(MasterDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }


        public async Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId)
        {
            var sql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
            return await _connection.QueryAsync<SubjectLanguage>(sql, new { SubjectId = subjectId });
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

        public Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage)
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

            _connection.Execute(sql, parameters);
            return Task.FromResult(subjectLanguage);
        }

        public Task DeleteAsync(SubjectLanguage subjectLanguage)
        {
            var sql = "EXEC [dbo].[SubjectLanguage_Delete] @Id";
            _connection.Execute(sql, new { Id = subjectLanguage.Id });
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
