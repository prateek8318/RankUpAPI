using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class UserSubscriptionDapperRepository : IUserSubscriptionRepository
    {
        private readonly SubscriptionDbContext _context;
        
        public UserSubscriptionDapperRepository(SubscriptionDbContext context)
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

        public async Task<UserSubscription?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { Id = id });
        }

        public async Task<IEnumerable<UserSubscription>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetAll]";
            return await connection.QueryAsync<UserSubscription>(sql);
        }

        public async Task<IEnumerable<UserSubscription>> FindAsync(System.Linq.Expressions.Expression<Func<UserSubscription, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(UserSubscription entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[UserSubscription_Create] 
                    @UserId, @SubscriptionPlanId, @Status, @StartDate, @EndDate, 
                    @RazorpayOrderId, @RazorpayPaymentId, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(UserSubscription entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[UserSubscription_Update] 
                    @Id, @UserId, @SubscriptionPlanId, @Status, @StartDate, @EndDate, 
                    @RazorpayOrderId, @RazorpayPaymentId, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(UserSubscription entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = entity.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetByUserId] @UserId";
            return await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<UserSubscription>> GetByUserIdWithHistoryAsync(int userId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetByUserIdWithHistory] @UserId";
            return await connection.QueryAsync<UserSubscription>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetActive]";
            return await connection.QueryAsync<UserSubscription>(sql);
        }

        public async Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetExpiring] @DaysBeforeExpiry";
            return await connection.QueryAsync<UserSubscription>(sql, new { DaysBeforeExpiry = daysBeforeExpiry });
        }

        public async Task<UserSubscription?> GetByRazorpayOrderIdAsync(string orderId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetByRazorpayOrderId] @OrderId";
            return await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { OrderId = orderId });
        }

        public async Task<UserSubscription?> GetByRazorpayPaymentIdAsync(string paymentId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserSubscription_GetByRazorpayPaymentId] @PaymentId";
            return await connection.QueryFirstOrDefaultAsync<UserSubscription>(sql, new { PaymentId = paymentId });
        }
    }
}
