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
                var row = await connection.QueryFirstOrDefaultAsync<SubscriptionPlanDbRow>(sql, new { Id = id });
                return row is null ? null : MapToEntity(row);
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetAll]";
                var rows = await connection.QueryAsync<SubscriptionPlanDbRow>(sql);
                return MapToEntityList(rows);
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
                        @Name, @Description, @Price, @Duration, @ValidityDays, @Type, @ExamCategory, @ExamId, @Features,
                        @IsActive, @SortOrder, @CreatedAt, @UpdatedAt";

                var parameters = new
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Duration = entity.Duration,
                    ValidityDays = entity.ValidityDays,
                    Type = (int)entity.Type,
                    ExamCategory = entity.ExamCategory,
                    ExamId = entity.ExamId ?? 0,
                    Features = entity.Features != null ? string.Join(",", entity.Features) : null,
                    IsActive = entity.IsActive,
                    SortOrder = entity.SortOrder,
                    CreatedAt = entity.CreatedAt == default ? DateTime.UtcNow : entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt ?? DateTime.UtcNow
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
                        @Id = @Id, 
                        @Name = @Name, 
                        @Description = @Description, 
                        @Price = @Price, 
                        @Duration = @Duration, 
                        @ValidityDays = @ValidityDays, 
                        @Type = @Type, 
                        @ExamCategory = @ExamCategory, 
                        @ExamId = @ExamId, 
                        @Features = @Features,
                        @IsActive = @IsActive, 
                        @SortOrder = @SortOrder, 
                        @IsPopular = @IsPopular, 
                        @IsRecommended = @IsRecommended, 
                        @CardColorTheme = @CardColorTheme, 
                        @UpdatedAt = @UpdatedAt";

                var parameters = new
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Duration = entity.Duration,
                    ValidityDays = entity.ValidityDays,
                    Type = (int)entity.Type,
                    ExamCategory = entity.ExamCategory,
                    ExamId = entity.ExamId ?? 0,
                    Features = entity.Features != null ? string.Join(",", entity.Features) : null,
                    IsActive = entity.IsActive,
                    SortOrder = entity.SortOrder,
                    IsPopular = entity.IsPopular,
                    IsRecommended = entity.IsRecommended,
                    CardColorTheme = entity.CardColorTheme,
                    UpdatedAt = DateTime.UtcNow
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
                var rows = await connection.QueryAsync<SubscriptionPlanDbRow>(sql, new { ExamCategory = examCategory });
                return MapToEntityList(rows);
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamIdAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetByExamId] @ExamId";
                var rows = await connection.QueryAsync<SubscriptionPlanDbRow>(sql, new { ExamId = examId });
                return MapToEntityList(rows);
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetActive]";
                var rows = await connection.QueryAsync<SubscriptionPlanDbRow>(sql);
                return MapToEntityList(rows);
            });
        }

        public async Task<(IEnumerable<SubscriptionPlan> Plans, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, bool includeInactive, int? examId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetAllPaged] @PageNumber, @PageSize, @IncludeInactive, @ExamId";
                var parameters = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    IncludeInactive = includeInactive,
                    ExamId = examId
                };

                using var grid = await connection.QueryMultipleAsync(sql, parameters);
                var planRows = await grid.ReadAsync<SubscriptionPlanDbRow>();
                var plans = MapToEntityList(planRows);
                var total = await grid.ReadFirstOrDefaultAsync<int>();

                return (plans, total);
            });
        }

        public async Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_GetByPlanType] @PlanType";
                var row = await connection.QueryFirstOrDefaultAsync<SubscriptionPlanDbRow>(sql, new { PlanType = (int)planType });
                return row is null ? null : MapToEntity(row);
            });
        }

        public async Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null, int? examId = null)
        {
            var result = await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubscriptionPlan_ExistsByName] @Name, @ExamCategory, @PlanType, @ExcludeId, @ExamId";
                return await connection.QuerySingleAsync<int>(sql, new
                {
                    Name = name,
                    ExamCategory = examCategory,
                    PlanType = (int)type,
                    ExcludeId = excludeId,
                    ExamId = examId
                });
            });
            return result > 0;
        }

        private static SubscriptionPlan MapToEntity(SubscriptionPlanDbRow row)
        {
            return new SubscriptionPlan
            {
                Id = row.Id,
                Name = row.Name,
                Description = row.Description,
                Type = row.Type,
                Price = row.Price,
                Currency = row.Currency,
                TestPapersCount = row.TestPapersCount,
                Discount = row.Discount,
                Duration = row.Duration,
                DurationType = row.DurationType,
                ValidityDays = row.ValidityDays,
                ExamId = row.ExamId,
                ExamCategory = row.ExamCategory,
                Features = ParseFeatures(row.Features),
                ImageUrl = row.ImageUrl,
                IsPopular = row.IsPopular,
                IsRecommended = row.IsRecommended,
                CardColorTheme = row.CardColorTheme,
                SortOrder = row.SortOrder,
                IsActive = row.IsActive,
                CreatedAt = row.CreatedAt,
                UpdatedAt = row.UpdatedAt
            };
        }

        private static List<string> ParseFeatures(string? features)
        {
            if (string.IsNullOrWhiteSpace(features))
            {
                return new List<string>();
            }

            var output = new List<string>();
            var parts = features.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var item = part.Trim();
                if (!string.IsNullOrWhiteSpace(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static List<SubscriptionPlan> MapToEntityList(IEnumerable<SubscriptionPlanDbRow> rows)
        {
            var plans = new List<SubscriptionPlan>();
            foreach (var row in rows)
            {
                plans.Add(MapToEntity(row));
            }
            return plans;
        }

        private sealed class SubscriptionPlanDbRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public PlanType Type { get; set; }
            public decimal Price { get; set; }
            public string Currency { get; set; } = "INR";
            public int TestPapersCount { get; set; }
            public decimal Discount { get; set; }
            public int Duration { get; set; }
            public string DurationType { get; set; } = "Monthly";
            public int ValidityDays { get; set; }
            public int? ExamId { get; set; }
            public string? ExamCategory { get; set; }
            public string? Features { get; set; }
            public string? ImageUrl { get; set; }
            public bool IsPopular { get; set; }
            public bool IsRecommended { get; set; }
            public string? CardColorTheme { get; set; }
            public int SortOrder { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
