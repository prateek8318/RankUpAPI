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
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<UserSubscription>(sql));
        }

        public async Task<IEnumerable<UserSubscription>> FindAsync(System.Linq.Expressions.Expression<Func<UserSubscription, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<UserSubscription> AddAsync(UserSubscription entity)
        {
            var sql = @"
                EXEC [dbo].[UserSubscription_Create] 
                    @UserId, @SubscriptionPlanId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
                    @OriginalAmount, @FinalAmount, @StartDate, @EndDate, @Status, @AutoRenew,
                    @RazorpaySubscriptionId, @CreatedAt, @UpdatedAt";

            var parameters = new
            {
                UserId = entity.UserId,
                SubscriptionPlanId = entity.SubscriptionPlanId,
                RazorpayOrderId = entity.RazorpayOrderId,
                RazorpayPaymentId = entity.RazorpayPaymentId,
                RazorpaySignature = entity.RazorpaySignature,
                // OriginalAmount = entity.OriginalAmount, // Not available
                // FinalAmount = entity.FinalAmount, // Not available
                // StartDate = entity.StartDate, // Not available
                // EndDate = entity.EndDate, // Not available
                Status = entity.Status,
                AutoRenew = entity.AutoRenewal,
                // RazorpaySubscriptionId = entity.RazorpaySubscriptionId, // Not available
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, parameters));
            return entity;
        }

        public async Task<UserSubscription> UpdateAsync(UserSubscription entity)
        {
            var sql = @"
                EXEC [dbo].[UserSubscription_Update] 
                    @Id, @UserId, @SubscriptionPlanId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
                    @OriginalAmount, @FinalAmount, @StartDate, @EndDate, @Status, @AutoRenew,
                    @RazorpaySubscriptionId, @LastRenewalDate, @CancelledDate, @CancellationReason, @UpdatedAt";

            var parameters = new
            {
                Id = entity.Id,
                UserId = entity.UserId,
                SubscriptionPlanId = entity.SubscriptionPlanId,
                RazorpayOrderId = entity.RazorpayOrderId,
                RazorpayPaymentId = entity.RazorpayPaymentId,
                RazorpaySignature = entity.RazorpaySignature,
                // OriginalAmount = entity.OriginalAmount, // Not available
                // FinalAmount = entity.FinalAmount, // Not available
                // StartDate = entity.StartDate, // Not available
                // EndDate = entity.EndDate, // Not available
                Status = entity.Status.ToString(),
                AutoRenewal = entity.AutoRenewal,
                // RazorpaySubscriptionId = entity.RazorpaySubscriptionId, // Not available
                // LastRenewalDate = entity.LastRenewalDate, // Not available
                // CancelledDate = entity.CancelledDate, // Not available
                // CancellationReason = entity.CancellationReason, // Not available
                UpdatedAt = entity.UpdatedAt
            };

            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, parameters));
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
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<UserSubscription>(sql, new { UserId = userId }));
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
                return await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { UserId = userId });
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
