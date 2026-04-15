using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Domain.Interfaces
{
    public interface IWalletRepository : ISubscriptionRepository<UserWallet>
    {
        Task<UserWallet?> GetByUserIdAsync(int userId);
        Task<UserWallet> GetOrCreateByUserIdAsync(int userId);
        Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<WalletTransaction?> GetTransactionByTransactionIdAsync(string transactionId);
        Task<WalletTransaction?> GetTransactionByProviderTransactionIdAsync(string providerTransactionId);
        Task<(IEnumerable<WalletTransaction> Transactions, int TotalCount)> GetTransactionsPagedAsync(int userId, int pageNumber, int pageSize, WalletTransactionType? transactionType = null, decimal? amountFrom = null, decimal? amountTo = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? referenceId = null);
        Task<(int TotalWallets, decimal TotalBalance, decimal AverageBalance, int ActiveWallets, int InactiveWallets, decimal TotalDeposits, decimal TotalWithdrawals, int WalletTransactionsToday, decimal TodayTransactionVolume)> GetWalletStatisticsAsync(int userId);
        Task<(int TotalWallets, decimal TotalBalance, decimal AverageBalance, int ActiveWallets, int InactiveWallets, decimal TotalDeposits, decimal TotalWithdrawals, int WalletTransactionsToday, decimal TodayTransactionVolume, int VerifiedWallets, int UnverifiedWallets, decimal PendingTransactions, decimal FailedTransactions)> GetAdminWalletStatisticsAsync();
        Task<(decimal CurrentBalance, decimal AvailableBalance, decimal BlockedAmount, bool HasSufficientBalance, string? Message)> CheckWalletBalanceAsync(int userId, decimal requiredAmount);
        Task<bool> UpdateWalletAsync(UserWallet wallet);
    }
}
