using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class ExamDapperRepository : BaseDapperRepository, IExamRepository
    {
        public ExamDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<ExamMaster?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamMaster_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<ExamMaster>(sql, new { Id = id });
            });
        }

        public async Task<ExamMaster?> GetByIdWithSubjectsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamMaster_GetByIdWithSubjects] @Id";
                return await connection.QueryFirstOrDefaultAsync<ExamMaster>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<ExamMaster>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamMaster_GetAll]";
                return await connection.QueryAsync<ExamMaster>(sql);
            });
        }

        public async Task<IEnumerable<ExamMaster>> GetActiveExamsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamMaster_GetActive]";
                return await connection.QueryAsync<ExamMaster>(sql);
            });
        }

        public async Task<IEnumerable<ExamMaster>> FindAsync(System.Linq.Expressions.Expression<Func<ExamMaster, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(ExamMaster entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[ExamMaster_Create] 
                        @Name, @Description, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(ExamMaster entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[ExamMaster_Update] 
                        @Id, @Name, @Description, @IsActive, @DisplayOrder, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(ExamMaster entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamMaster_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
