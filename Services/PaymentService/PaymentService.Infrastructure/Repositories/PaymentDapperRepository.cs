using Dapper;
using Microsoft.Data.SqlClient;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using System.Data;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentDapperRepository : BaseDapperRepository, IPaymentRepository
    {
        public PaymentDapperRepository(string connectionString) : base(connectionString)
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

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByTransactionId] @TransactionId";
                return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { TransactionId = transactionId });
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

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Payment_GetByUserId] @UserId";
                return await connection.QueryAsync<Payment>(sql, new { UserId = userId });
            });
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Payment_Create] 
                        @UserId, @Amount, @Currency, @Status, @PaymentMethod,
                        @TransactionId, @Provider, @ProviderTransactionId, @Description,
                        @Metadata, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", payment.UserId);
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@Currency", payment.Currency);
                parameters.Add("@Status", payment.Status);
                parameters.Add("@PaymentMethod", payment.PaymentMethod);
                parameters.Add("@TransactionId", payment.TransactionId);
                parameters.Add("@Provider", payment.Provider);
                parameters.Add("@ProviderTransactionId", payment.ProviderTransactionId);
                parameters.Add("@Description", payment.Description);
                parameters.Add("@Metadata", payment.Metadata);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    payment.Id = parameters.Get<int>("@Id");
                }

                return payment;
            });
        }

        public async Task UpdateAsync(Payment payment)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Payment_Update] 
                        @Id, @UserId, @Amount, @Currency, @Status, @PaymentMethod,
                        @TransactionId, @Provider, @ProviderTransactionId, @Description,
                        @Metadata, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", payment.Id);
                parameters.Add("@UserId", payment.UserId);
                parameters.Add("@Amount", payment.Amount);
                parameters.Add("@Currency", payment.Currency);
                parameters.Add("@Status", payment.Status);
                parameters.Add("@PaymentMethod", payment.PaymentMethod);
                parameters.Add("@TransactionId", payment.TransactionId);
                parameters.Add("@Provider", payment.Provider);
                parameters.Add("@ProviderTransactionId", payment.ProviderTransactionId);
                parameters.Add("@Description", payment.Description);
                parameters.Add("@Metadata", payment.Metadata);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
