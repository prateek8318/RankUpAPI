using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectDapperRepository : ISubjectRepository
    {
        private readonly MasterDbContext _context;

        public SubjectDapperRepository(MasterDbContext context)
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

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Subject_GetAll]";
            var subjects = await connection.QueryAsync<Subject>(sql);

            // Load SubjectLanguages for each subject
            var subjectList = subjects.ToList();
            foreach (var subject in subjectList)
            {
                var languages = await connection.QueryAsync<SubjectLanguage>(
                    "SELECT sl.*, l.Code as LanguageCode, l.Name as LanguageName FROM SubjectLanguages sl " +
                    "LEFT JOIN Languages l ON sl.LanguageId = l.Id " +
                    "WHERE sl.SubjectId = @SubjectId AND sl.IsActive = 1",
                    new { SubjectId = subject.Id });
                
                subject.SubjectLanguages = languages.ToList();
            }

            return subjectList;
        }

        public async Task<Subject> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Subject_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Subject>(sql, new { Id = id }) ?? new Subject();
        }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Subject_GetActive]";
            return await connection.QueryAsync<Subject>(sql);
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Subject_Create] 
                    @Name, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", subject.Name);
            parameters.Add("@IsActive", subject.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                subject.Id = parameters.Get<int>("@Id");
            }

            return subject;
        }

        public Task<Subject> UpdateAsync(Subject subject)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = @"
                EXEC [dbo].[Subject_Update] 
                    @Id, @Name, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", subject.Id);
            parameters.Add("@Name", subject.Name);
            parameters.Add("@IsActive", subject.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            connection.Execute(sql, parameters);
            return Task.FromResult(subject);
        }

        public Task DeleteAsync(Subject subject)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[Subject_Delete] @Id";
            connection.Execute(sql, new { Id = subject.Id });
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Subject_Exists] @Id";
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
            return result > 0;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
