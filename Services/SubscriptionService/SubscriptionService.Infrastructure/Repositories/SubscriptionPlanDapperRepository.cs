using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class SubscriptionPlanDapperRepository : BaseDapperRepository, ISubscriptionPlanRepository
    {
        public SubscriptionPlanDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetAll]";
                return await connection.QueryAsync<SubscriptionPlan>(sql);
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> FindAsync(System.Linq.Expressions.Expression<Func<SubscriptionPlan, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[SubscriptionPlan_Create] 
                        @Name, @Description, @Price, @Duration, @Type, @ExamCategory, 
                        @ExamId, @Features, @IsActive, @SortOrder, @CreatedAt, @UpdatedAt";

                var parameters = new
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Duration = entity.Duration,
                    Type = (int)entity.Type,
                    ExamCategory = entity.ExamCategory,
                    ExamId = entity.ExamId,
                    Features = entity.Features != null ? string.Join(",", entity.Features) : null,
                    IsActive = entity.IsActive,
                    SortOrder = entity.SortOrder,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                };

                await connection.ExecuteAsync(sql, parameters);
                return entity;
            });
        }

        public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[SubscriptionPlan_Update] 
                        @Id, @Name, @Description, @Price, @Duration, @Type, @ExamCategory, 
                        @ExamId, @Features, @IsActive, @SortOrder, @UpdatedAt";

                var parameters = new
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Duration = entity.Duration,
                    Type = (int)entity.Type,
                    ExamCategory = entity.ExamCategory,
                    ExamId = entity.ExamId,
                    Features = entity.Features != null ? string.Join(",", entity.Features) : null,
                    IsActive = entity.IsActive,
                    SortOrder = entity.SortOrder,
                    UpdatedAt = entity.UpdatedAt
                };

                await connection.ExecuteAsync(sql, parameters);
                return entity;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affectedRows = await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_Delete] @Id";
                return await connection.ExecuteAsync(sql, new { Id = id });
            });
            return affectedRows > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamCategoryAsync(string examCategory)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetByExamCategory] @ExamCategory";
                return await connection.QueryAsync<SubscriptionPlan>(sql, new { ExamCategory = examCategory });
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamIdAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetByExamId] @ExamId";
                return await connection.QueryAsync<SubscriptionPlan>(sql, new { ExamId = examId });
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetActive]";
                return await connection.QueryAsync<SubscriptionPlan>(sql);
            });
        }

        public async Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetByPlanType] @PlanType";
                return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(sql, new { PlanType = planType });
            });
        }

        public async Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null, int? examId = null)
        {
            var result = await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_ExistsByName] @Name, @ExamCategory, @PlanType, @ExcludeId, @ExamId";
                return await connection.QueryFirstOrDefaultAsync<int>(sql, new { 
                    Name = name, 
                    ExamCategory = examCategory, 
                    PlanType = type, 
                    ExcludeId = excludeId, 
                    ExamId = examId 
                });
            });
            return result > 0;
        }
    }
}
