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

        // New methods for duration options support
        public async Task<SubscriptionPlan?> GetPlanWithDurationsAsync(int id, string languageCode = "en")
        {
            return await WithConnectionAsync(async connection =>
            {
                // Get subscription plan with translations
                var planSql = @"
                    SELECT 
                        sp.Id,
                        sp.Name,
                        sp.Description,
                        sp.Type,
                        sp.Price,
                        sp.Currency,
                        sp.TestPapersCount,
                        sp.Discount,
                        sp.Duration,
                        sp.DurationType,
                        sp.ValidityDays,
                        sp.ExamId,
                        sp.ExamCategory,
                        sp.Features,
                        sp.ImageUrl,
                        sp.IsPopular,
                        sp.IsRecommended,
                        sp.CardColorTheme,
                        sp.SortOrder,
                        sp.CreatedAt,
                        sp.UpdatedAt,
                        sp.IsActive,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Name
                            ELSE sp.Name
                        END AS LocalizedName,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Description
                            ELSE sp.Description
                        END AS LocalizedDescription,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Features
                            ELSE sp.Features
                        END AS LocalizedFeatures
                    FROM SubscriptionPlans sp
                    LEFT JOIN SubscriptionPlanTranslations spt ON sp.Id = spt.SubscriptionPlanId AND spt.LanguageCode = @LanguageCode
                    WHERE sp.Id = @PlanId";

                var planRow = await connection.QueryFirstOrDefaultAsync<SubscriptionPlanWithDurationsDbRow>(planSql, new { PlanId = id, LanguageCode = languageCode });
                if (planRow == null) return null;

                // Get duration options for this plan
                var durationSql = @"
                    SELECT 
                        pdo.Id,
                        pdo.SubscriptionPlanId,
                        pdo.DurationMonths,
                        pdo.Price,
                        pdo.DiscountPercentage,
                        pdo.DisplayLabel,
                        pdo.IsPopular,
                        pdo.SortOrder,
                        pdo.IsActive,
                        pdo.CreatedAt,
                        pdo.UpdatedAt
                    FROM PlanDurationOptions pdo
                    WHERE pdo.SubscriptionPlanId = @PlanId AND pdo.IsActive = 1
                    ORDER BY pdo.SortOrder";

                var durationRows = await connection.QueryAsync<PlanDurationOptionDbRow>(durationSql, new { PlanId = id });
                
                var plan = MapToEntityWithDurations(planRow, durationRows);
                return plan;
            });
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansWithDurationsAsync(string languageCode = "en", int? examId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Get active subscription plans with translations
                var planSql = @"
                    SELECT 
                        sp.Id,
                        sp.Name,
                        sp.Description,
                        sp.Type,
                        sp.Price,
                        sp.Currency,
                        sp.TestPapersCount,
                        sp.Discount,
                        sp.Duration,
                        sp.DurationType,
                        sp.ValidityDays,
                        sp.ExamId,
                        sp.ExamCategory,
                        sp.Features,
                        sp.ImageUrl,
                        sp.IsPopular,
                        sp.IsRecommended,
                        sp.CardColorTheme,
                        sp.SortOrder,
                        sp.CreatedAt,
                        sp.UpdatedAt,
                        sp.IsActive,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Name
                            ELSE sp.Name
                        END AS LocalizedName,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Description
                            ELSE sp.Description
                        END AS LocalizedDescription,
                        CASE 
                            WHEN spt.LanguageCode = @LanguageCode THEN spt.Features
                            ELSE sp.Features
                        END AS LocalizedFeatures
                    FROM SubscriptionPlans sp
                    LEFT JOIN SubscriptionPlanTranslations spt ON sp.Id = spt.SubscriptionPlanId AND spt.LanguageCode = @LanguageCode
                    WHERE sp.IsActive = 1 
                    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL)
                    ORDER BY sp.SortOrder, sp.Name";

                var planRows = await connection.QueryAsync<SubscriptionPlanWithDurationsDbRow>(planSql, new { LanguageCode = languageCode, ExamId = examId });

                // Get duration options for these plans
                var durationSql = @"
                    SELECT 
                        pdo.Id,
                        pdo.SubscriptionPlanId,
                        pdo.DurationMonths,
                        pdo.Price,
                        pdo.DiscountPercentage,
                        pdo.DisplayLabel,
                        pdo.IsPopular,
                        pdo.SortOrder,
                        pdo.IsActive,
                        pdo.CreatedAt,
                        pdo.UpdatedAt
                    FROM PlanDurationOptions pdo
                    INNER JOIN SubscriptionPlans sp ON pdo.SubscriptionPlanId = sp.Id
                    WHERE sp.IsActive = 1 
                    AND pdo.IsActive = 1
                    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL)
                    ORDER BY pdo.SubscriptionPlanId, pdo.SortOrder";

                var durationRows = await connection.QueryAsync<PlanDurationOptionDbRow>(durationSql, new { ExamId = examId });
                
                var plans = new List<SubscriptionPlan>();
                foreach (var planRow in planRows)
                {
                    var planDurations = durationRows.Where(d => d.SubscriptionPlanId == planRow.Id).ToList();
                    var plan = MapToEntityWithDurations(planRow, planDurations);
                    plans.Add(plan);
                }
                
                return plans;
            });
        }

        public async Task<PlanDurationOption?> GetDurationOptionAsync(int durationOptionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT Id, SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
                           DisplayLabel, IsPopular, SortOrder, IsActive, CreatedAt, UpdatedAt
                    FROM PlanDurationOptions 
                    WHERE Id = @Id AND IsActive = 1";
                
                var row = await connection.QueryFirstOrDefaultAsync<PlanDurationOptionDbRow>(sql, new { Id = durationOptionId });
                return row == null ? null : MapToDurationOption(row);
            });
        }

        public async Task AddDurationOptionAsync(PlanDurationOption durationOption)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO PlanDurationOptions (
                        SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
                        DisplayLabel, IsPopular, SortOrder, IsActive, CreatedAt
                    ) VALUES (
                        @SubscriptionPlanId, @DurationMonths, @Price, @DiscountPercentage,
                        @DisplayLabel, @IsPopular, @SortOrder, @IsActive, @CreatedAt
                    )";
                
                await connection.ExecuteAsync(sql, durationOption);
            });
        }

        public async Task UpdateDurationOptionAsync(PlanDurationOption durationOption)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE PlanDurationOptions 
                    SET DurationMonths = @DurationMonths,
                        Price = @Price,
                        DiscountPercentage = @DiscountPercentage,
                        DisplayLabel = @DisplayLabel,
                        IsPopular = @IsPopular,
                        SortOrder = @SortOrder,
                        IsActive = @IsActive,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(sql, durationOption);
            });
        }

        public async Task<UserSubscription?> GetUserActiveSubscriptionAsync(int userId, int planId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT TOP 1 Id, UserId, SubscriptionPlanId, PaymentId, PurchasedDate, ValidTill,
                           TestsUsed, TestsTotal, AmountPaid, Currency, DiscountApplied, Status,
                           AutoRenewal, RenewalDate, CreatedAt, UpdatedAt, IsActive
                    FROM UserSubscriptions 
                    WHERE UserId = @UserId AND SubscriptionPlanId = @PlanId 
                    AND Status = 'Active' AND ValidTill > GETDATE() AND IsActive = 1";
                
                var row = await connection.QueryFirstOrDefaultAsync<UserSubscriptionDbRow>(sql, new { UserId = userId, PlanId = planId });
                return row == null ? null : MapToUserSubscription(row);
            });
        }

        public async Task<IEnumerable<UserSubscription>> GetUserActiveSubscriptionsAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT Id, UserId, SubscriptionPlanId, PaymentId, PurchasedDate, ValidTill,
                           TestsUsed, TestsTotal, AmountPaid, Currency, DiscountApplied, Status,
                           AutoRenewal, RenewalDate, CreatedAt, UpdatedAt, IsActive
                    FROM UserSubscriptions 
                    WHERE UserId = @UserId 
                    AND Status = 'Active' AND ValidTill > GETDATE() AND IsActive = 1";
                
                var rows = await connection.QueryAsync<UserSubscriptionDbRow>(sql, new { UserId = userId });
                return rows.Select(MapToUserSubscription);
            });
        }

        public async Task AddTranslationAsync(SubscriptionPlanTranslation translation)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO SubscriptionPlanTranslations (
                        SubscriptionPlanId, LanguageCode, Name, Description, Features, CreatedAt
                    ) VALUES (
                        @SubscriptionPlanId, @LanguageCode, @Name, @Description, @Features, @CreatedAt
                    )";
                
                await connection.ExecuteAsync(sql, translation);
            });
        }

        public async Task<SubscriptionPlan> CreatePlanWithDurationsAsync(SubscriptionPlan plan, IEnumerable<PlanDurationOption> durationOptions, IEnumerable<SubscriptionPlanTranslation>? translations = null)
        {
            return await WithTransactionAsync(async (connection, transaction) =>
            {
                // Create the subscription plan
                var planSql = @"
                    INSERT INTO SubscriptionPlans (
                        Name, Description, Price, Duration, ValidityDays, Type, ExamCategory, ExamId, Features,
                        IsActive, SortOrder, CreatedAt, UpdatedAt
                    ) VALUES (
                        @Name, @Description, @Price, @Duration, @ValidityDays, @Type, @ExamCategory, @ExamId, @Features,
                        @IsActive, @SortOrder, @CreatedAt, @UpdatedAt
                    );
                    
                    SELECT CAST(SCOPE_IDENTITY() as int) as Id, 
                           Name, Description, Price, Duration, ValidityDays, Type, ExamCategory, ExamId, Features,
                           IsActive, SortOrder, CreatedAt, UpdatedAt, Currency, TestPapersCount, Discount,
                           DurationType, ImageUrl, IsPopular, IsRecommended, CardColorTheme
                    FROM SubscriptionPlans WHERE Id = CAST(SCOPE_IDENTITY() as int);";

                var planParameters = new
                {
                    plan.Name,
                    plan.Description,
                    plan.Price,
                    plan.Duration,
                    plan.ValidityDays,
                    plan.Type,
                    plan.ExamCategory,
                    plan.ExamId,
                    Features = plan.Features != null ? string.Join(",", plan.Features) : null,
                    plan.IsActive,
                    plan.SortOrder,
                    plan.CreatedAt,
                    plan.UpdatedAt
                };

                var createdPlan = await connection.QueryFirstOrDefaultAsync<SubscriptionPlanDbRow>(planSql, planParameters, transaction);
                if (createdPlan == null)
                    throw new InvalidOperationException("Failed to create subscription plan");

                var mappedPlan = MapToEntity(createdPlan);

                // Add duration options
                foreach (var option in durationOptions)
                {
                    var durationOption = new PlanDurationOption
                    {
                        SubscriptionPlanId = mappedPlan.Id,
                        DurationMonths = option.DurationMonths,
                        Price = option.Price,
                        DiscountPercentage = option.DiscountPercentage,
                        DisplayLabel = option.DisplayLabel,
                        IsPopular = option.IsPopular,
                        SortOrder = option.SortOrder,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var durationSql = @"
                        INSERT INTO PlanDurationOptions (
                            SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
                            DisplayLabel, IsPopular, SortOrder, IsActive, CreatedAt
                        ) VALUES (
                            @SubscriptionPlanId, @DurationMonths, @Price, @DiscountPercentage,
                            @DisplayLabel, @IsPopular, @SortOrder, @IsActive, @CreatedAt
                        )";
                    
                    await connection.ExecuteAsync(durationSql, durationOption, transaction);
                }

                // Add translations if provided
                if (translations != null)
                {
                    foreach (var translation in translations)
                    {
                        var translationEntity = new SubscriptionPlanTranslation
                        {
                            SubscriptionPlanId = mappedPlan.Id,
                            LanguageCode = translation.LanguageCode,
                            Name = translation.Name,
                            Description = translation.Description,
                            Features = translation.Features
                        };

                        var translationSql = @"
                            INSERT INTO SubscriptionPlanTranslations (
                                SubscriptionPlanId, LanguageCode, Name, Description, Features, CreatedAt
                            ) VALUES (
                                @SubscriptionPlanId, @LanguageCode, @Name, @Description, @Features, @CreatedAt
                            )";
                        
                        await connection.ExecuteAsync(translationSql, translationEntity, transaction);
                    }
                }

                return mappedPlan;
            });
        }

        private static SubscriptionPlan MapToEntityWithDurations(SubscriptionPlanWithDurationsDbRow row, IEnumerable<PlanDurationOptionDbRow> durationRows)
        {
            var plan = new SubscriptionPlan
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
                UpdatedAt = row.UpdatedAt,
                DurationOptions = durationRows.Select(MapToDurationOption).ToList()
            };

            return plan;
        }

        private static PlanDurationOption MapToDurationOption(PlanDurationOptionDbRow row)
        {
            return new PlanDurationOption
            {
                Id = row.Id,
                SubscriptionPlanId = row.SubscriptionPlanId,
                DurationMonths = row.DurationMonths,
                Price = row.Price,
                DiscountPercentage = row.DiscountPercentage,
                DisplayLabel = row.DisplayLabel,
                IsPopular = row.IsPopular,
                SortOrder = row.SortOrder,
                IsActive = row.IsActive,
                CreatedAt = row.CreatedAt,
                UpdatedAt = row.UpdatedAt
            };
        }

        private static UserSubscription MapToUserSubscription(UserSubscriptionDbRow row)
        {
            return new UserSubscription
            {
                Id = row.Id,
                UserId = row.UserId,
                SubscriptionPlanId = row.SubscriptionPlanId,
                PaymentId = row.PaymentId,
                PurchasedDate = row.PurchasedDate,
                ValidTill = row.ValidTill,
                TestsUsed = row.TestsUsed,
                TestsTotal = row.TestsTotal,
                AmountPaid = row.AmountPaid,
                Currency = row.Currency,
                DiscountApplied = row.DiscountApplied,
                Status = row.Status,
                AutoRenewal = row.AutoRenewal,
                RenewalDate = row.RenewalDate,
                CreatedAt = row.CreatedAt,
                UpdatedAt = row.UpdatedAt,
                IsActive = row.IsActive
            };
        }

        private sealed class SubscriptionPlanWithDurationsDbRow : SubscriptionPlanDbRow
        {
            public string? LocalizedName { get; set; }
            public string? LocalizedDescription { get; set; }
            public string? LocalizedFeatures { get; set; }
        }

        private sealed class PlanDurationOptionDbRow
        {
            public int Id { get; set; }
            public int SubscriptionPlanId { get; set; }
            public int DurationMonths { get; set; }
            public decimal Price { get; set; }
            public decimal DiscountPercentage { get; set; }
            public string DisplayLabel { get; set; } = string.Empty;
            public bool IsPopular { get; set; }
            public int SortOrder { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        private sealed class UserSubscriptionDbRow
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int SubscriptionPlanId { get; set; }
            public int? PaymentId { get; set; }
            public DateTime PurchasedDate { get; set; }
            public DateTime ValidTill { get; set; }
            public int TestsUsed { get; set; }
            public int TestsTotal { get; set; }
            public decimal AmountPaid { get; set; }
            public string Currency { get; set; } = "INR";
            public decimal DiscountApplied { get; set; }
            public string Status { get; set; } = string.Empty;
            public bool AutoRenewal { get; set; }
            public DateTime? RenewalDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsActive { get; set; }
        }

        private class SubscriptionPlanDbRow
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
