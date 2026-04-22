using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ExamService.Application.Interfaces;
using ExamService.Application.DTOs;
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

        public async Task<IEnumerable<Exam>> GetAllIncludingInactiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetAllIncludingInactive]";
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
                        @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt,
                        @ExamCategoryId, @ExamTypeId, @SubjectId, @TotalQuestions, @MarksPerQuestion,
                        @HasNegativeMarking, @NegativeMarkingValue, @AccessType, @SubscriptionPlanId,
                        @ExamDate, @PublishDateTime, @ValidTill, @AttemptsAllowed, @ShowResultType, @Status,
                        @DurationInMinutes, @TotalMarks, @PassingMarks, @ImageUrl, @IsInternational,
                        @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Name", exam.Name);
                parameters.Add("@Description", exam.Description);
                parameters.Add("@IsActive", exam.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@ExamCategoryId", exam.ExamCategoryId);
                parameters.Add("@ExamTypeId", exam.ExamTypeId);
                parameters.Add("@SubjectId", exam.SubjectId);
                parameters.Add("@TotalQuestions", exam.TotalQuestions);
                parameters.Add("@MarksPerQuestion", exam.MarksPerQuestion);
                parameters.Add("@HasNegativeMarking", exam.HasNegativeMarking);
                parameters.Add("@NegativeMarkingValue", exam.NegativeMarkingValue);
                parameters.Add("@AccessType", exam.AccessType);
                parameters.Add("@SubscriptionPlanId", exam.SubscriptionPlanId);
                parameters.Add("@ExamDate", exam.ExamDate);
                parameters.Add("@PublishDateTime", exam.PublishDateTime);
                parameters.Add("@ValidTill", exam.ValidTill);
                parameters.Add("@AttemptsAllowed", exam.AttemptsAllowed);
                parameters.Add("@ShowResultType", exam.ShowResultType);
                parameters.Add("@Status", exam.Status);
                parameters.Add("@DurationInMinutes", exam.DurationInMinutes);
                parameters.Add("@TotalMarks", exam.TotalMarks);
                parameters.Add("@PassingMarks", exam.PassingMarks);
                parameters.Add("@ImageUrl", exam.ImageUrl);
                parameters.Add("@IsInternational", exam.IsInternational);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    exam.Id = parameters.Get<int>("@Id");
                }

                return exam;
            });
        }

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var transaction = connection.BeginTransaction();
                return transaction as IDbTransaction;
            });
        }

        public async Task CommitAsync(IDbTransaction transaction)
        {
            await WithConnectionAsync(async connection =>
            {
                if (transaction is SqlTransaction sqlTransaction)
                {
                    sqlTransaction.Commit();
                }
            });
        }

        public async Task RollbackAsync(IDbTransaction transaction)
        {
            await WithConnectionAsync(async connection =>
            {
                if (transaction is SqlTransaction sqlTransaction)
                {
                    sqlTransaction.Rollback();
                }
            });
        }

        public async Task UpdateAsync(Exam exam)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Exam_Update] 
                        @Id, @Name, @Description, @IsActive, @UpdatedAt,
                        @ExamCategoryId, @ExamTypeId, @SubjectId, @TotalQuestions, @MarksPerQuestion,
                        @HasNegativeMarking, @NegativeMarkingValue, @AccessType, @SubscriptionPlanId,
                        @ExamDate, @PublishDateTime, @ValidTill, @AttemptsAllowed, @ShowResultType, @Status,
                        @DurationInMinutes, @TotalMarks, @PassingMarks, @ImageUrl, @IsInternational";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", exam.Id);
                parameters.Add("@Name", exam.Name);
                parameters.Add("@Description", exam.Description);
                parameters.Add("@IsActive", exam.IsActive);
                parameters.Add("@UpdatedAt", exam.UpdatedAt ?? DateTime.UtcNow);
                parameters.Add("@ExamCategoryId", exam.ExamCategoryId);
                parameters.Add("@ExamTypeId", exam.ExamTypeId);
                parameters.Add("@SubjectId", exam.SubjectId);
                parameters.Add("@TotalQuestions", exam.TotalQuestions);
                parameters.Add("@MarksPerQuestion", exam.MarksPerQuestion);
                parameters.Add("@HasNegativeMarking", exam.HasNegativeMarking);
                parameters.Add("@NegativeMarkingValue", exam.NegativeMarkingValue);
                parameters.Add("@AccessType", exam.AccessType);
                parameters.Add("@SubscriptionPlanId", exam.SubscriptionPlanId);
                parameters.Add("@ExamDate", exam.ExamDate);
                parameters.Add("@PublishDateTime", exam.PublishDateTime);
                parameters.Add("@ValidTill", exam.ValidTill);
                parameters.Add("@AttemptsAllowed", exam.AttemptsAllowed);
                parameters.Add("@ShowResultType", exam.ShowResultType);
                parameters.Add("@Status", exam.Status);
                parameters.Add("@DurationInMinutes", exam.DurationInMinutes);
                parameters.Add("@TotalMarks", exam.TotalMarks);
                parameters.Add("@PassingMarks", exam.PassingMarks);
                parameters.Add("@ImageUrl", exam.ImageUrl);
                parameters.Add("@IsInternational", exam.IsInternational);

                await connection.ExecuteAsync(sql, parameters);
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

        // Admin specific methods
        public async Task<ExamStatsDto> GetExamStatsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetStats]";
                var stats = await connection.QuerySingleAsync<ExamStatsDto>(sql);
                return stats;
            });
        }

        public async Task<IEnumerable<ExamCategory>> GetActiveCategoriesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamCategory_GetActive]";
                return await connection.QueryAsync<ExamCategory>(sql);
            });
        }

        public async Task<IEnumerable<ExamCategory>> GetExamCategoriesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamCategory_GetAll]";
                return await connection.QueryAsync<ExamCategory>(sql);
            });
        }

        public async Task<IEnumerable<ExamType>> GetTypesByCategoryIdAsync(int categoryId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamType_GetByCategoryId] @ExamCategoryId";
                return await connection.QueryAsync<ExamType>(sql, new { ExamCategoryId = categoryId });
            });
        }

        public async Task<IEnumerable<ExamType>> GetExamTypesByCategoryAsync(int categoryId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ExamType_GetByCategoryId] @ExamCategoryId";
                return await connection.QueryAsync<ExamType>(sql, new { ExamCategoryId = categoryId });
            });
        }

        public async Task<IEnumerable<Exam>> GetFilteredExamsAsync(int? categoryId, int? typeId, string? status)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetFiltered] @ExamCategoryId, @ExamTypeId, @Status";
                return await connection.QueryAsync<Exam>(sql, new { ExamCategoryId = categoryId, ExamTypeId = typeId, Status = status });
            });
        }

        public async Task<bool> UpdateExamStatusAsync(int id, string status)
        {
            var affectedRows = await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_UpdateStatus] @Id, @Status";
                return await connection.ExecuteAsync(sql, new { Id = id, Status = status });
            });
            return affectedRows > 0;
        }

        public async Task<ExamDashboardDto> GetExamDashboardAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetDashboard]";
                using var multi = await connection.QueryMultipleAsync(sql);
                
                var stats = await multi.ReadSingleAsync<ExamStatsDto>();
                var recentExams = await multi.ReadAsync<RecentExamDto>();
                var categoryDistribution = await multi.ReadAsync<CategoryDistributionDto>();
                
                return new ExamDashboardDto
                {
                    Stats = stats,
                    RecentExams = recentExams.ToList(),
                    CategoryDistribution = categoryDistribution.ToList()
                };
            });
        }
    }
}
