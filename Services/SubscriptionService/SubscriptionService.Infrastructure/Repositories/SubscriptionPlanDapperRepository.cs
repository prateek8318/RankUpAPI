using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class SubscriptionPlanDapperRepository : ISubscriptionPlanRepository
    {
        private readonly SubscriptionDbContext _context;
        
        public SubscriptionPlanDapperRepository(SubscriptionDbContext context)
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

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(sql, new { Id = id });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetAll]";
            return await connection.QueryAsync<SubscriptionPlan>(sql);
        }

        public async Task<IEnumerable<SubscriptionPlan>> FindAsync(System.Linq.Expressions.Expression<Func<SubscriptionPlan, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[SubscriptionPlan_Create] 
                    @Name, @Description, @Price, @Duration, @Type, @ExamCategory, 
                    @ExamId, @Features, @IsActive, @SortOrder, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[SubscriptionPlan_Update] 
                    @Id, @Name, @Description, @Price, @Duration, @Type, @ExamCategory, 
                    @ExamId, @Features, @IsActive, @SortOrder, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_Delete] @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamCategoryAsync(string examCategory)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetByExamCategory] @ExamCategory";
            return await connection.QueryAsync<SubscriptionPlan>(sql, new { ExamCategory = examCategory });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamIdAsync(int examId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetByExamId] @ExamId";
            return await connection.QueryAsync<SubscriptionPlan>(sql, new { ExamId = examId });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetActive]";
            return await connection.QueryAsync<SubscriptionPlan>(sql);
        }

        public async Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_GetByPlanType] @PlanType";
            return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>(sql, new { PlanType = planType });
        }

        public async Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null, int? examId = null)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[SubscriptionPlan_ExistsByName] @Name, @ExamCategory, @PlanType, @ExcludeId, @ExamId";
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { 
                Name = name, 
                ExamCategory = examCategory, 
                PlanType = type, 
                ExcludeId = excludeId, 
                ExamId = examId 
            });
            return result > 0;
        }
    }
}
