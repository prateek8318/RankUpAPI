using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class UserSubscriptionDapperRepository : BaseDapperRepository, IUserSubscriptionRepository
    {
        public UserSubscriptionDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<UserSubscription?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetById] @Id";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { Id = id }));
        }

        public async Task<IEnumerable<UserSubscription>> GetAllAsync()
        {
            var sql = "EXEC [dbo].[UserSubscription_GetAll]";
            var fallbackSql = @"
                SELECT
                    us.Id,
                    us.UserId,
                    us.SubscriptionPlanId,
                    us.DurationOptionId,
                    us.RazorpayOrderId,
                    us.RazorpayPaymentId,
                    us.RazorpaySignature,
                    us.PurchasedDate,
                    us.ValidTill,
                    us.TestsUsed,
                    us.TestsTotal,
                    us.AmountPaid,
                    us.Currency,
                    us.DiscountApplied,
                    us.Status,
                    us.AutoRenewal,
                    us.CreatedAt,
                    us.UpdatedAt
                FROM [dbo].[UserSubscriptions] us
                ORDER BY us.CreatedAt DESC";

            return await WithConnectionAsync(async connection =>
            {
                try
                {
                    return await connection.QueryAsync<UserSubscription>(sql);
                }
                catch (SqlException)
                {
                    return await connection.QueryAsync<UserSubscription>(fallbackSql);
                }
            });
        }

        public async Task<IEnumerable<UserSubscription>> FindAsync(System.Linq.Expressions.Expression<Func<UserSubscription, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<UserSubscription> AddAsync(UserSubscription entity)
        {
            var parameters = new
            {
                UserId = entity.UserId,
                SubscriptionPlanId = entity.SubscriptionPlanId,
                DurationOptionId = entity.DurationOptionId ?? 0,
                RazorpayOrderId = entity.RazorpayOrderId,
                RazorpayPaymentId = entity.RazorpayPaymentId ?? (string?)null,
                RazorpaySignature = entity.RazorpaySignature ?? (string?)null,
                OriginalAmount = entity.AmountPaid + entity.DiscountApplied,
                FinalAmount = entity.AmountPaid,
                StartDate = entity.PurchasedDate,
                EndDate = entity.ValidTill,
                Status = entity.Status,
                AutoRenewal = entity.AutoRenewal,
                RazorpaySubscriptionId = (string?)null,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            var sqlWithDurationOption = @"
                EXEC [dbo].[UserSubscription_Create] 
                    @UserId = @UserId,
                    @SubscriptionPlanId = @SubscriptionPlanId,
                    @DurationOptionId = @DurationOptionId,
                    @RazorpayOrderId = @RazorpayOrderId,
                    @RazorpayPaymentId = @RazorpayPaymentId,
                    @RazorpaySignature = @RazorpaySignature,
                    @OriginalAmount = @OriginalAmount,
                    @FinalAmount = @FinalAmount,
                    @StartDate = @StartDate,
                    @EndDate = @EndDate,
                    @Status = @Status,
                    @AutoRenewal = @AutoRenewal,
                    @RazorpaySubscriptionId = @RazorpaySubscriptionId,
                    @CreatedAt = @CreatedAt,
                    @UpdatedAt = @UpdatedAt";

            var sqlLegacy = @"
                EXEC [dbo].[UserSubscription_Create] 
                    @UserId = @UserId,
                    @SubscriptionPlanId = @SubscriptionPlanId,
                    @RazorpayOrderId = @RazorpayOrderId,
                    @RazorpayPaymentId = @RazorpayPaymentId,
                    @RazorpaySignature = @RazorpaySignature,
                    @OriginalAmount = @OriginalAmount,
                    @FinalAmount = @FinalAmount,
                    @StartDate = @StartDate,
                    @EndDate = @EndDate,
                    @Status = @Status,
                    @AutoRenewal = @AutoRenewal,
                    @RazorpaySubscriptionId = @RazorpaySubscriptionId,
                    @CreatedAt = @CreatedAt,
                    @UpdatedAt = @UpdatedAt";

            var sqlDirectInsert = @"
                INSERT INTO [dbo].[UserSubscriptions]
                (
                    [UserId],
                    [SubscriptionPlanId],
                    [DurationOptionId],
                    [RazorpayOrderId],
                    [RazorpayPaymentId],
                    [RazorpaySignature],
                    [PurchasedDate],
                    [ValidTill],
                    [TestsUsed],
                    [TestsTotal],
                    [AmountPaid],
                    [Currency],
                    [DiscountApplied],
                    [Status],
                    [AutoRenewal],
                    [CreatedAt],
                    [UpdatedAt]
                )
                VALUES
                (
                    @UserId,
                    @SubscriptionPlanId,
                    @DurationOptionId,
                    @RazorpayOrderId,
                    @RazorpayPaymentId,
                    @RazorpaySignature,
                    @StartDate,
                    @EndDate,
                    0,
                    0,
                    @FinalAmount,
                    'INR',
                    (@OriginalAmount - @FinalAmount),
                    @Status,
                    @AutoRenewal,
                    @CreatedAt,
                    @UpdatedAt
                );";

            await WithConnectionAsync(async connection =>
            {
                try
                {
                    await connection.ExecuteAsync(sqlWithDurationOption, parameters);
                }
                catch (SqlException ex) when (ex.Number == 8144)
                {
                    // Backward compatibility for environments where UserSubscription_Create
                    // has not yet been updated with @DurationOptionId.
                    try
                    {
                        await connection.ExecuteAsync(sqlLegacy, parameters);
                    }
                    catch (SqlException)
                    {
                        // Final fallback for environments with incompatible proc signatures.
                        await connection.ExecuteAsync(sqlDirectInsert, parameters);
                    }
                }
            });
            return entity;
        }

        public async Task<UserSubscription> UpdateAsync(UserSubscription entity)
        {
            var procedureSql = @"
                EXEC [dbo].[UserSubscription_Update] 
                    @Id, @UserId, @SubscriptionPlanId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
                    @OriginalAmount, @FinalAmount, @StartDate, @EndDate, @Status, @AutoRenew,
                    @RazorpaySubscriptionId, @LastRenewalDate, @CancelledDate, @CancellationReason, @UpdatedAt";

            var fallbackSql = @"
                UPDATE [dbo].[UserSubscriptions]
                SET
                    [UserId] = @UserId,
                    [SubscriptionPlanId] = @SubscriptionPlanId,
                    [DurationOptionId] = @DurationOptionId,
                    [RazorpayOrderId] = @RazorpayOrderId,
                    [RazorpayPaymentId] = @RazorpayPaymentId,
                    [RazorpaySignature] = @RazorpaySignature,
                    [StartDate] = @StartDate,
                    [EndDate] = @EndDate,
                    [TestsUsed] = @TestsUsed,
                    [TestsTotal] = @TestsTotal,
                    [AmountPaid] = @AmountPaid,
                    [Currency] = @Currency,
                    [DiscountApplied] = @DiscountApplied,
                    [Status] = @Status,
                    [AutoRenewal] = @AutoRenewal,
                    [CancelledDate] = @CancelledDate,
                    [CancellationReason] = @CancellationReason,
                    [UpdatedAt] = @UpdatedAt
                WHERE [Id] = @Id";

            var parameters = new
            {
                Id = entity.Id,
                UserId = entity.UserId,
                SubscriptionPlanId = entity.SubscriptionPlanId,
                DurationOptionId = entity.DurationOptionId,
                RazorpayOrderId = entity.RazorpayOrderId,
                RazorpayPaymentId = entity.RazorpayPaymentId,
                RazorpaySignature = entity.RazorpaySignature,
                OriginalAmount = entity.AmountPaid + entity.DiscountApplied,
                FinalAmount = entity.AmountPaid,
                StartDate = entity.PurchasedDate > DateTime.MinValue ? entity.PurchasedDate : DateTime.UtcNow,
                EndDate = entity.ValidTill > DateTime.MinValue ? entity.ValidTill : DateTime.UtcNow.AddYears(1),
                Status = entity.Status,
                AutoRenewal = entity.AutoRenewal,
                RazorpaySubscriptionId = (string?)null,
                LastRenewalDate = (DateTime?)null,
                CancelledDate = entity.Status == "Cancelled" ? DateTime.UtcNow : (DateTime?)null,
                CancellationReason = (string?)null,
                UpdatedAt = entity.UpdatedAt ?? DateTime.UtcNow,
                TestsUsed = entity.TestsUsed,
                TestsTotal = entity.TestsTotal,
                AmountPaid = entity.AmountPaid,
                Currency = entity.Currency,
                DiscountApplied = entity.DiscountApplied
            };

            await WithConnectionAsync(async connection =>
            {
                try
                {
                    await connection.ExecuteAsync(procedureSql, parameters);
                }
                catch (SqlException)
                {
                    // Fallback for environments where proc signature differs from current entity model.
                    await connection.ExecuteAsync(fallbackSql, parameters);
                }
            });
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "EXEC [dbo].[UserSubscription_Delete] @Id";
            var affectedRows = await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, new { Id = id }));
            return affectedRows > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetByUserId] @UserId";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { UserId = userId }));
        }

        public async Task<IEnumerable<UserSubscription>> GetByUserIdWithHistoryAsync(int userId)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetByUserIdWithHistory] @UserId";
            var fallbackSql = @"
                SELECT
                    us.Id,
                    us.UserId,
                    us.SubscriptionPlanId,
                    us.DurationOptionId,
                    us.RazorpayOrderId,
                    us.RazorpayPaymentId,
                    us.RazorpaySignature,
                    us.PurchasedDate,
                    us.ValidTill,
                    us.TestsUsed,
                    us.TestsTotal,
                    us.AmountPaid,
                    us.Currency,
                    us.DiscountApplied,
                    us.Status,
                    us.AutoRenewal,
                    us.CreatedAt,
                    us.UpdatedAt
                FROM [dbo].[UserSubscriptions] us
                WHERE us.UserId = @UserId
                ORDER BY us.CreatedAt DESC";

            return await WithConnectionAsync(async connection =>
            {
                try
                {
                    return await connection.QueryAsync<UserSubscription>(sql, new { UserId = userId });
                }
                catch (SqlException)
                {
                    return await connection.QueryAsync<UserSubscription>(fallbackSql, new { UserId = userId });
                }
            });
        }

        public async Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync()
        {
            var sql = "EXEC [dbo].[UserSubscription_GetActive]";
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<UserSubscription>(sql));
        }

        public async Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetExpiring] @DaysBeforeExpiry";
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<UserSubscription>(sql, new { DaysBeforeExpiry = daysBeforeExpiry }));
        }

        public async Task<UserSubscription?> GetByRazorpayOrderIdAsync(string orderId)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetByRazorpayOrderId] @OrderId";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { OrderId = orderId }));
        }

        public async Task<UserSubscription?> GetByRazorpayPaymentIdAsync(string paymentId)
        {
            var sql = "EXEC [dbo].[UserSubscription_GetByRazorpayPaymentId] @PaymentId";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { PaymentId = paymentId }));
        }

        public async Task<IEnumerable<UserSubscription>> GetByPaymentIdAsync(int paymentId)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return empty result
                return new List<UserSubscription>();
            });
        }

        public async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserSubscription_GetActiveByUserId] @UserId";
                var fromProcedure = await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { UserId = userId });
                if (fromProcedure != null)
                {
                    return fromProcedure;
                }

                // Fallback for environments where stored procedure/schema is out of sync.
                // Prefer an active, non-expired subscription; otherwise return latest non-cancelled subscription.
                var fallbackSql = @"
                    SELECT TOP 1
                        us.Id,
                        us.UserId,
                        us.SubscriptionPlanId,
                        us.DurationOptionId,
                        us.RazorpayOrderId,
                        us.RazorpayPaymentId,
                        us.RazorpaySignature,
                        us.PurchasedDate,
                        us.ValidTill,
                        us.TestsUsed,
                        us.TestsTotal,
                        us.AmountPaid,
                        us.Currency,
                        us.DiscountApplied,
                        us.Status,
                        us.AutoRenewal,
                        us.CreatedAt,
                        us.UpdatedAt
                    FROM [dbo].[UserSubscriptions] us
                    WHERE us.UserId = @UserId
                      AND us.Status <> 'Cancelled'
                    ORDER BY
                        CASE
                            WHEN us.Status = 'Active' AND us.ValidTill >= GETUTCDATE() THEN 0
                            WHEN us.Status = 'Pending' THEN 1
                            ELSE 2
                        END,
                        us.CreatedAt DESC";

                return await connection.QueryFirstOrDefaultAsync<UserSubscription>(fallbackSql, new { UserId = userId });
            });
        }

        public async Task<(IEnumerable<UserSubscription> Subscriptions, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? userId = null, string? searchTerm = null, SubscriptionStatus? status = null, bool? autoRenewal = null, DateTime? startDateFrom = null, DateTime? startDateTo = null, DateTime? endDateFrom = null, DateTime? endDateTo = null, bool? includeExpired = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return empty result
                return (new List<UserSubscription>(), 0);
            });
        }
    }
}