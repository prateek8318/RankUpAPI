using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(CreateRazorpayOrderDto createOrderDto);
        Task<PaymentVerificationResultDto> VerifyPaymentAsync(VerifyPaymentDto verifyPaymentDto);
        Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto);
        Task<PaymentTransactionDto?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<IEnumerable<PaymentTransactionDto>> GetUserPaymentHistoryAsync(int userId);
    }
}
