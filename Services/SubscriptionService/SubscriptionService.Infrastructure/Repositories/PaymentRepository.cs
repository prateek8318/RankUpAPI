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

        public async Task<SubscriptionStatisticsDto> GetStatisticsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetStatistics]";
                var result = await connection.QueryFirstOrDefaultAsync<PaymentStatisticsRow>(sql);
                
                // Debug: Log the result
                System.Diagnostics.Debug.WriteLine($"PaymentStatisticsRow result: TotalPayments={result?.TotalPayments}, ActivePlansCount={result?.ActivePlansCount}, ActiveSubscribers={result?.ActiveSubscribers}");
                if (result == null)
                {
                    return new SubscriptionStatisticsDto
                    {
                        TotalPayments = 0,
                        TotalRevenue = 0m,
                        TodayRevenue = 0m,
                        ThisMonthRevenue = 0m,
                        SuccessfulPayments = 0,
                        FailedPayments = 0,
                        PendingPayments = 0,
                        AverageTransactionAmount = 0m,
                        UniquePayingUsers = 0,
                        RefundedAmount = 0m,
                        RefundedPayments = 0,
                        ActivePlansCount = 0,
                        ActiveSubscribers = 0,
                        ExpiringSoon = 0,
                        NewSubscribersToday = 0,
                        NewUsersLast7Days = 0,
                        BlockedUsers = 0,
                        TotalUsers = 0,
                        FreeUsers = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                }

                return new SubscriptionStatisticsDto
                {
                    TotalPayments = result.TotalPayments,
                    TotalRevenue = result.TotalRevenue,
                    TodayRevenue = result.TodayRevenue,
                    ThisMonthRevenue = result.ThisMonthRevenue,
                    SuccessfulPayments = result.SuccessfulPayments,
                    FailedPayments = result.FailedPayments,
                    PendingPayments = result.PendingPayments,
                    AverageTransactionAmount = result.AverageTransactionAmount,
                    UniquePayingUsers = result.UniquePayingUsers,
                    RefundedAmount = result.RefundedAmount,
                    RefundedPayments = result.RefundedPayments,
                    ActivePlansCount = 20, // Hardcoded as requested
                    ActiveSubscribers = result.ActiveSubscribers,
                    ExpiringSoon = result.ExpiringSoon,
                    NewSubscribersToday = result.NewSubscribersToday,
                    NewUsersLast7Days = result.NewUsersLast7Days,
                    BlockedUsers = result.BlockedUsers,
                    TotalUsers = result.TotalUsers,
                    FreeUsers = result.FreeUsers,
                    LastUpdated = DateTime.UtcNow
                };
            });
        }

        private sealed class PaymentStatisticsRow
        {
            // Payment Statistics
            public int TotalPayments { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TodayRevenue { get; set; }
            public decimal ThisMonthRevenue { get; set; }
            public int SuccessfulPayments { get; set; }
            public int FailedPayments { get; set; }
            public int PendingPayments { get; set; }
            public decimal AverageTransactionAmount { get; set; }
            public int UniquePayingUsers { get; set; }
            public decimal RefundedAmount { get; set; }
            public int RefundedPayments { get; set; }
            
            // Plan Statistics
            public int ActivePlansCount { get; set; }
            
            // User Subscription Statistics
            public int ActiveSubscribers { get; set; }
            public int ExpiringSoon { get; set; }
            public int NewUsersLast7Days { get; set; }
            public int BlockedUsers { get; set; }
            public int TotalUsers { get; set; }
            public int FreeUsers { get; set; }
            public int NewSubscribersToday { get; set; }
            
            public DateTime LastUpdated { get; set; }
        }
    }
}
