using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface IWalletService
    {
        Task<UserWalletDto?> GetUserWalletAsync(int userId);
        Task<UserWalletDto> GetOrCreateUserWalletAsync(int userId);
        Task<WalletTransactionListResponseDto> GetWalletTransactionsAsync(int userId, WalletTransactionListRequestDto request);
        Task<WalletStatisticsDto> GetWalletStatisticsAsync(int userId);
        Task<AdminWalletStatisticsDto> GetAdminWalletStatisticsAsync();
        Task<RazorpayWalletRechargeResponseDto> InitiateWalletRechargeAsync(RechargeWalletRazorpayDto rechargeDto);
        Task<WalletRechargeVerificationResultDto> VerifyWalletRechargeAsync(VerifyWalletRechargeDto verifyDto);
        Task<WalletPaymentResultDto> PayWithWalletAsync(PayWithWalletDto payDto);
        Task<WalletBalanceCheckResultDto> CheckWalletBalanceAsync(CheckWalletBalanceDto checkDto);
        Task<bool> BlockWalletAsync(int userId, string reason);
        Task<bool> UnblockWalletAsync(int userId);
        Task<WalletTransactionDto?> GetWalletTransactionByIdAsync(int transactionId);
        Task<WalletTransactionDto?> GetWalletTransactionByProviderIdAsync(string providerTransactionId);
    }
}
