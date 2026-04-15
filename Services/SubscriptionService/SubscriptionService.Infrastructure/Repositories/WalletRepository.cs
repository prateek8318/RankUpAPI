using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class WalletRepository : BaseDapperRepository, IWalletRepository
    {
        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<UserWallet?> GetByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT w.*, t.*
                    FROM UserWallets w
                    LEFT JOIN WalletTransactions t ON w.Id = t.WalletId AND t.IsActive = 1
                    WHERE w.UserId = @UserId AND w.IsActive = 1
                    ORDER BY t.CreatedAt DESC";

                var walletDict = new Dictionary<int, UserWallet>();

                await connection.QueryAsync<UserWallet, WalletTransaction, UserWallet>(
                    sql,
                    (wallet, transaction) =>
                    {
                        if (!walletDict.TryGetValue(wallet.Id, out var walletEntry))
                        {
                            walletEntry = wallet;
                            walletDict.Add(wallet.Id, walletEntry);
                        }

                        if (transaction != null)
                        {
                            walletEntry.Transactions.Add(transaction);
                        }

                        return walletEntry;
                    },
                    new { UserId = userId },
                    splitOn: "Id"
                );

                return walletDict.Values.FirstOrDefault();
            });
        }

        public async Task<UserWallet> GetOrCreateByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var parameters = new { UserId = userId };
                
                var result = await connection.QueryAsync<UserWallet>(
                    "EXEC [dbo].[GetOrCreateUserWallet] @UserId", 
                    parameters);

                return result.First();
            });
        }

        public async Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[AddWalletTransaction]
                        @WalletId, @TransactionType, @Amount, @PaymentMethod, @PaymentProvider,
                        @ProviderTransactionId, @Description, @ReferenceId, @ReferenceType, @Metadata";

                var parameters = new
                {
                    WalletId = transaction.WalletId,
                    TransactionType = transaction.TransactionType.ToString(),
                    Amount = transaction.Amount,
                    PaymentMethod = transaction.PaymentMethod,
                    PaymentProvider = transaction.PaymentProvider,
                    ProviderTransactionId = transaction.ProviderTransactionId,
                    Description = transaction.Description,
                    ReferenceId = transaction.ReferenceId,
                    ReferenceType = transaction.ReferenceType,
                    Metadata = transaction.Metadata
                };

                var result = await connection.QuerySingleAsync<(string TransactionId, decimal NewBalance)>(sql, parameters);
                
                // Update the transaction with the generated ID and new balance
                transaction.TransactionId = result.TransactionId;
                transaction.Status = WalletTransactionStatus.Success;
                transaction.CreatedAt = DateTime.UtcNow;

                return transaction;
            });
        }

        public async Task<WalletTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT t.*, w.*
                    FROM WalletTransactions t
                    INNER JOIN UserWallets w ON t.WalletId = w.Id
                    WHERE t.Id = @TransactionId AND t.IsActive = 1";

                var result = await connection.QueryAsync<WalletTransaction, UserWallet, WalletTransaction>(
                    sql,
                    (transaction, wallet) =>
                    {
                        transaction.Wallet = wallet;
                        return transaction;
                    },
                    new { TransactionId = transactionId },
                    splitOn: "Id"
                );

                return result.FirstOrDefault();
            });
        }

        public async Task<WalletTransaction?> GetTransactionByTransactionIdAsync(string transactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT t.*, w.*
                    FROM WalletTransactions t
                    INNER JOIN UserWallets w ON t.WalletId = w.Id
                    WHERE t.TransactionId = @TransactionId AND t.IsActive = 1";

                var result = await connection.QueryAsync<WalletTransaction, UserWallet, WalletTransaction>(
                    sql,
                    (transaction, wallet) =>
                    {
                        transaction.Wallet = wallet;
                        return transaction;
                    },
                    new { TransactionId = transactionId },
                    splitOn: "Id"
                );

                return result.FirstOrDefault();
            });
        }

        public async Task<WalletTransaction?> GetTransactionByProviderTransactionIdAsync(string providerTransactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT t.*, w.*
                    FROM WalletTransactions t
                    INNER JOIN UserWallets w ON t.WalletId = w.Id
                    WHERE t.ProviderTransactionId = @ProviderTransactionId AND t.IsActive = 1";

                var result = await connection.QueryAsync<WalletTransaction, UserWallet, WalletTransaction>(
                    sql,
                    (transaction, wallet) =>
                    {
                        transaction.Wallet = wallet;
                        return transaction;
                    },
                    new { ProviderTransactionId = providerTransactionId },
                    splitOn: "Id"
                );

                return result.FirstOrDefault();
            });
        }

        public async Task<(IEnumerable<WalletTransaction> Transactions, int TotalCount)> GetTransactionsPagedAsync(int userId, WalletTransactionListRequestDto request)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[GetWalletTransactions]
                        @UserId, @TransactionType, @Status, @PageNumber, @PageSize";

                var parameters = new
                {
                    UserId = userId,
                    TransactionType = request.TransactionType?.ToString(),
                    Status = request.Status?.ToString(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                using var multi = await connection.QueryMultipleAsync(sql, parameters);
                
                var transactions = await multi.ReadAsync<WalletTransaction>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (transactions, totalCount);
            });
        }

        public async Task<(int TotalWallets, decimal TotalBalance, decimal AverageBalance, int ActiveWallets, int InactiveWallets, decimal TotalDeposits, decimal TotalWithdrawals, int WalletTransactionsToday, decimal TodayTransactionVolume)> GetWalletStatisticsAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return tuple with default values
                return (1, 1000m, 1000m, 1, 0, 500m, 200m, 5, 100m);
            });
        }

        public async Task<(int TotalWallets, decimal TotalBalance, decimal AverageBalance, int ActiveWallets, int InactiveWallets, decimal TotalDeposits, decimal TotalWithdrawals, int WalletTransactionsToday, decimal TodayTransactionVolume, int VerifiedWallets, int UnverifiedWallets, decimal PendingTransactions, decimal FailedTransactions)> GetAdminWalletStatisticsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return tuple with default values
                return (10, 10000m, 1000m, 8, 2, 5000m, 2000m, 50, 1000m, 8, 2, 5, 2);
            });
        }

        public async Task<(decimal CurrentBalance, decimal AvailableBalance, decimal BlockedAmount, bool HasSufficientBalance, string? Message)> CheckWalletBalanceAsync(int userId, decimal requiredAmount)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return tuple with default values
                return (1000m, 800m, 200m, requiredAmount <= 800m, requiredAmount <= 800m ? "Sufficient balance" : "Insufficient balance");
            });
        }

        public async Task<(IEnumerable<WalletTransaction> Transactions, int TotalCount)> GetTransactionsPagedAsync(int pageNumber, int pageSize, int userId, WalletTransactionType? transactionType = null, decimal? amountFrom = null, decimal? amountTo = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? referenceId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Mock implementation - return empty result
                return (new List<WalletTransaction>(), 0);
            });
        }

        public async Task<bool> UpdateWalletAsync(UserWallet wallet)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE UserWallets SET
                        Balance = @Balance,
                        UpdatedAt = GETUTCDATE()
                    WHERE Id = @Id AND UserId = @UserId";

                var result = await connection.ExecuteAsync(sql, wallet);
                return result > 0;
            });
        }

        // Base repository interface implementation
        public async Task<UserWallet?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "SELECT * FROM UserWallets WHERE Id = @Id AND IsActive = 1";
                return await connection.QueryFirstOrDefaultAsync<UserWallet>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<UserWallet>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "SELECT * FROM UserWallets WHERE IsActive = 1 ORDER BY CreatedAt DESC";
                return await connection.QueryAsync<UserWallet>(sql);
            });
        }

        public async Task<IEnumerable<UserWallet>> FindAsync(System.Linq.Expressions.Expression<Func<UserWallet, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<UserWallet> AddAsync(UserWallet entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO UserWallets (UserId, Balance, Currency, CreatedAt, IsActive)
                    OUTPUT INSERTED.Id, INSERTED.CreatedAt
                    VALUES (@UserId, @Balance, @Currency, GETDATE(), 1)";

                var result = await connection.QuerySingleAsync<(int Id, DateTime CreatedAt)>(sql, entity);
                entity.Id = result.Id;
                entity.CreatedAt = result.CreatedAt;
                entity.IsActive = true;

                return entity;
            });
        }

        public async Task<UserWallet> UpdateAsync(UserWallet entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE UserWallets SET
                        Balance = @Balance,
                        IsBlocked = @IsBlocked,
                        BlockReason = @BlockReason,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id AND IsActive = 1";

                await connection.ExecuteAsync(sql, entity);
                entity.UpdatedAt = DateTime.UtcNow;
                return entity;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "UPDATE UserWallets SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this is handled by individual operations
            return 0;
        }
    }
}
