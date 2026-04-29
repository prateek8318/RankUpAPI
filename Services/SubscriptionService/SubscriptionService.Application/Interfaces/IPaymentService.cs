using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentInitiationResponseDto> InitiatePaymentAsync(InitiatePaymentDto initiatePaymentDto);
        Task<PaymentVerificationResultDto> VerifyPaymentAsync(VerifyPaymentDto verifyPaymentDto);
        Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto);
        Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<IEnumerable<PaymentDto>> GetUserPaymentHistoryAsync(int userId);
        Task<SubscriptionStatisticsDto> GetStatisticsAsync();
        Task<(IEnumerable<PaymentDto> Payments, int TotalCount)> GetPagedPaymentsAsync(AdminPaymentListRequestDto request);
    }
}
