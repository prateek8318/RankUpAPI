using Dapper;
using Microsoft.Data.SqlClient;
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

        protected SqlConnection GetConnection()
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
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetByIdWithQualifications] @Id";
            return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
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

        public async Task AddAsync(Exam exam)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Exam_Create] 
                    @Name, @Description, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, exam);
        }

        public async Task UpdateAsync(Exam exam)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Exam_Update] 
                    @Id, @Name, @Description, @IsActive, @DisplayOrder, @UpdatedAt";

            await connection.ExecuteAsync(sql, exam);
        }

        public async Task DeleteAsync(Exam exam)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = exam.Id });
        }

        public async Task<bool> HardDeleteByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_HardDeleteById] @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
