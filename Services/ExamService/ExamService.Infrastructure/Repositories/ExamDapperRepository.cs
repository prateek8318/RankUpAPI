using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;

namespace ExamService.Infrastructure.Repositories
{
    public class ExamDapperRepository : BaseDapperRepository, IExamRepository
    {
        public ExamDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
            });
        }

        public async Task<Exam?> GetByIdWithQualificationsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetByIdWithQualifications] @Id";
                return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetAll]";
                return await connection.QueryAsync<Exam>(sql);
            });
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetActive]";
                return await connection.QueryAsync<Exam>(sql);
            });
        }

        public async Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetByQualificationId] @QualificationId";
                return await connection.QueryAsync<Exam>(sql, new { QualificationId = qualificationId });
            });
        }

        public async Task<IEnumerable<Exam>> GetByQualificationAndStreamAsync(int qualificationId, int? streamId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetByQualificationAndStream] @QualificationId, @StreamId";
                return await connection.QueryAsync<Exam>(sql, new { QualificationId = qualificationId, StreamId = streamId });
            });
        }

        public async Task<Exam> AddAsync(Exam exam)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Exam_Create] 
                        @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Name", exam.Name);
                parameters.Add("@Description", exam.Description);
                parameters.Add("@IsActive", exam.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    exam.Id = parameters.Get<int>("@Id");
                }

                return exam;
            });
        }

        public async Task UpdateAsync(Exam exam)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Exam_Update] 
                        @Id, @Name, @Description, @IsActive, @UpdatedAt";

                await connection.ExecuteAsync(sql, exam);
            });
        }

        public async Task DeleteAsync(Exam exam)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = exam.Id });
            });
        }

        public async Task<bool> HardDeleteByIdAsync(int id)
        {
            var affectedRows = await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_HardDeleteById] @Id";
                return await connection.ExecuteAsync(sql, new { Id = id });
            });
            return affectedRows > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
