using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using ExamService.Infrastructure.Data;

namespace ExamService.Infrastructure.Repositories
{
    public class ExamDapperRepository : IExamRepository
    {
        private readonly ExamDbContext _context;
        
        public ExamDapperRepository(ExamDbContext context)
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

        public async Task<Exam?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
        }

        public async Task<Exam?> GetByIdWithQualificationsAsync(int id)
        {
            // For complex queries with includes, create separate SPs
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetAll]";
            return await connection.QueryAsync<Exam>(sql);
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetActive]";
            return await connection.QueryAsync<Exam>(sql);
        }

        public async Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetByQualificationId] @QualificationId";
            return await connection.QueryAsync<Exam>(sql, new { QualificationId = qualificationId });
        }

        public async Task<IEnumerable<Exam>> GetByQualificationAndStreamAsync(int qualificationId, int? streamId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetByQualificationAndStream] @QualificationId, @StreamId";
            return await connection.QueryAsync<Exam>(sql, new { QualificationId = qualificationId, StreamId = streamId });
        }

        public async Task<Exam> AddAsync(Exam exam)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Exam_Insert] 
                    @Name, @Description, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, exam);
            return exam;
        }

        public Task UpdateAsync(Exam exam)
        {
            // For updates, create separate SP if needed
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Exam exam)
        {
            // For deletes, create separate SP if needed
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
