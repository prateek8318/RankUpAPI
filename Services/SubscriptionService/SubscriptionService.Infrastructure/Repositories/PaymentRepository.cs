using Dapper;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class PaymentRepository : BaseDapperRepository, IPaymentRepository
    {
        public PaymentRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetAll]";
                return await connection.QueryAsync<Payment>(sql);
            });
        }

        public Task<IEnumerable<Payment>> FindAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<Payment> AddAsync(Payment entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"EXEC [dbo].[Payment_Create]
                            @UserId, @SubscriptionPlanId, @UserSubscriptionId, @Amount, @Currency, @DiscountAmount, @FinalAmount,
                            @PaymentMethod, @PaymentProvider, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
                            @Status, @PaymentDate, @FailureReason, @RefundAmount, @RefundDate, @RefundReason, @RazorpayRefundId, @Metadata";

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", entity.UserId);
                parameters.Add("@SubscriptionPlanId", entity.SubscriptionPlanId);
                parameters.Add("@UserSubscriptionId", entity.UserSubscriptionId);
                parameters.Add("@Amount", entity.Amount);
                parameters.Add("@Currency", entity.Currency);
                parameters.Add("@DiscountAmount", entity.DiscountAmount);
                parameters.Add("@FinalAmount", entity.FinalAmount);
                parameters.Add("@PaymentMethod", entity.PaymentMethod);
                parameters.Add("@PaymentProvider", entity.PaymentProvider);
                parameters.Add("@RazorpayOrderId", entity.RazorpayOrderId);
                parameters.Add("@RazorpayPaymentId", entity.RazorpayPaymentId);
                parameters.Add("@RazorpaySignature", entity.RazorpaySignature);
                parameters.Add("@Status", entity.Status);
                parameters.Add("@PaymentDate", entity.PaymentDate);
                parameters.Add("@FailureReason", entity.FailureReason);
                parameters.Add("@RefundAmount", entity.RefundAmount);
                parameters.Add("@RefundDate", entity.RefundDate);
                parameters.Add("@RefundReason", entity.RefundReason);
                parameters.Add("@RazorpayRefundId", entity.RazorpayRefundId);
                parameters.Add("@Metadata", entity.Metadata);

                var createdId = await connection.QuerySingleAsync<int>(sql, parameters);

                entity.Id = createdId;
                entity.IsActive = true;
                if (entity.CreatedAt == default)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                return entity;
            });
        }

        public async Task<Payment> UpdateAsync(Payment entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"EXEC [dbo].[Payment_Update]
                            @Id, @UserId, @SubscriptionPlanId, @UserSubscriptionId, @Amount, @Currency, @DiscountAmount, @FinalAmount,
                            @PaymentMethod, @PaymentProvider, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
                            @Status, @PaymentDate, @FailureReason, @RefundAmount, @RefundDate, @RefundReason, @RazorpayRefundId, @Metadata";

                await connection.ExecuteAsync(sql, new
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    SubscriptionPlanId = entity.SubscriptionPlanId,
                    UserSubscriptionId = entity.UserSubscriptionId,
                    Amount = entity.Amount,
                    Currency = entity.Currency,
                    DiscountAmount = entity.DiscountAmount,
                    FinalAmount = entity.FinalAmount,
                    PaymentMethod = entity.PaymentMethod,
                    PaymentProvider = entity.PaymentProvider,
                    RazorpayOrderId = entity.RazorpayOrderId,
                    RazorpayPaymentId = entity.RazorpayPaymentId,
                    RazorpaySignature = entity.RazorpaySignature,
                    Status = entity.Status,
                    PaymentDate = entity.PaymentDate,
                    FailureReason = entity.FailureReason,
                    RefundAmount = entity.RefundAmount,
                    RefundDate = entity.RefundDate,
                    RefundReason = entity.RefundReason,
                    RazorpayRefundId = entity.RazorpayRefundId,
                    Metadata = entity.Metadata
                });
                entity.UpdatedAt = DateTime.UtcNow;
                return entity;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_Delete] @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this is handled by individual operations
            return 0;
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByTransactionId] @TransactionId";
                return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { TransactionId = transactionId });
            });
        }

        public async Task<Payment?> GetByProviderOrderIdAsync(string providerOrderId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByProviderOrderId] @ProviderOrderId";
                return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { ProviderOrderId = providerOrderId });
            });
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByUserId] @UserId";
                return await connection.QueryAsync<Payment>(sql, new { UserId = userId });
            });
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByStatus] @Status";
                return await connection.QueryAsync<Payment>(sql, new { Status = status });
            });
        }

        public async Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? userId = null, string? searchTerm = null, PaymentStatus? status = null, PaymentMethod? method = null, decimal? amountFrom = null, decimal? amountTo = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? reference = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"EXEC [dbo].[Payment_GetPaged]
                            @PageNumber, @PageSize, @UserId, @SearchTerm, @Status, @PaymentMethod,
                            @AmountFrom, @AmountTo, @DateFrom, @DateTo, @Reference";
                using var grid = await connection.QueryMultipleAsync(sql, new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    UserId = userId,
                    SearchTerm = searchTerm,
                    Status = status,
                    PaymentMethod = method,
                    AmountFrom = amountFrom,
                    AmountTo = amountTo,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Reference = reference
                });
                var payments = await grid.ReadAsync<Payment>();
                var totalCount = await grid.ReadFirstOrDefaultAsync<int>();
                return (payments, totalCount);
            });
        }

        public async Task<(int TotalPayments, decimal TotalRevenue, decimal TodayRevenue, decimal ThisMonthRevenue, int SuccessfulPayments, int FailedPayments, int PendingPayments, decimal AverageTransactionAmount, int UniquePayingUsers)> GetStatisticsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetStatistics]";
                var result = await connection.QueryFirstOrDefaultAsync<PaymentStatisticsRow>(sql);
                if (result == null)
                {
                    return (0, 0m, 0m, 0m, 0, 0, 0, 0m, 0);
                }

                return (
                    result.TotalPayments,
                    result.TotalRevenue,
                    result.TodayRevenue,
                    result.ThisMonthRevenue,
                    result.SuccessfulPayments,
                    result.FailedPayments,
                    result.PendingPayments,
                    result.AverageTransactionAmount,
                    result.UniquePayingUsers);
            });
        }

        private sealed class PaymentStatisticsRow
        {
            public int TotalPayments { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TodayRevenue { get; set; }
            public decimal ThisMonthRevenue { get; set; }
            public int SuccessfulPayments { get; set; }
            public int FailedPayments { get; set; }
            public int PendingPayments { get; set; }
            public decimal AverageTransactionAmount { get; set; }
            public int UniquePayingUsers { get; set; }
        }
    }
}
