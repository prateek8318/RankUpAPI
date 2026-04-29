using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PaymentInitiationResponseDto> InitiatePaymentAsync(InitiatePaymentDto initiatePaymentDto)
        {
            try
            {
                _logger.LogInformation("Initiating UPI payment for user: {UserId}, plan: {PlanId}, provider: {Provider}", 
                    initiatePaymentDto.UserId, initiatePaymentDto.PlanId, initiatePaymentDto.PaymentProvider);

                // Get subscription plan
                var plan = await _subscriptionPlanRepository.GetByIdAsync(initiatePaymentDto.PlanId);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {initiatePaymentDto.PlanId} not found");
                }

                // Calculate final amount after discount
                var finalAmount = plan.Price - (plan.Price * plan.Discount / 100);

                // Create payment record
                var payment = new Payment
                {
                    UserId = initiatePaymentDto.UserId,
                    SubscriptionPlanId = initiatePaymentDto.PlanId,
                    Amount = plan.Price,
                    DiscountAmount = plan.Price * plan.Discount / 100,
                    FinalAmount = finalAmount,
                    Currency = initiatePaymentDto.Currency,
                    PaymentMethod = initiatePaymentDto.PaymentMethod,
                    PaymentProvider = initiatePaymentDto.PaymentProvider.ToString(),
                    Status = PaymentStatus.Pending,
                    Metadata = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        PlanName = plan.Name,
                        PlanType = plan.Type,
                        InitiatedAt = DateTime.UtcNow
                    })
                };

                var createdPayment = await _paymentRepository.AddAsync(payment);

                // Generate provider order ID based on payment provider
                var providerOrderId = await GenerateProviderOrderIdAsync(initiatePaymentDto.PaymentProvider, createdPayment.Id);

                // Update payment with provider order ID
                createdPayment.RazorpayOrderId = providerOrderId;
                await _paymentRepository.UpdateAsync(createdPayment);

                // Generate payment URL and QR code based on provider
                var (paymentUrl, qrCode) = await GeneratePaymentDetailsAsync(initiatePaymentDto.PaymentProvider, providerOrderId, finalAmount, initiatePaymentDto.Currency);

                return new PaymentInitiationResponseDto
                {
                    OrderId = providerOrderId,
                    Amount = finalAmount,
                    Currency = initiatePaymentDto.Currency,
                    Receipt = $"RCPT_{createdPayment.Id}",
                    PaymentProvider = initiatePaymentDto.PaymentProvider,
                    CreatedAt = createdPayment.CreatedAt,
                    PaymentUrl = paymentUrl,
                    QrCode = qrCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating UPI payment for user: {UserId}, plan: {PlanId}", 
                    initiatePaymentDto.UserId, initiatePaymentDto.PlanId);
                throw;
            }
        }

        public async Task<PaymentVerificationResultDto> VerifyPaymentAsync(VerifyPaymentDto verifyPaymentDto)
        {
            try
            {
                _logger.LogInformation("Verifying UPI payment for transaction: {TransactionId}, provider: {Provider}", 
                    verifyPaymentDto.TransactionId, verifyPaymentDto.PaymentProvider);

                // Get payment by transaction ID
                var payment = await _paymentRepository.GetByTransactionIdAsync(verifyPaymentDto.TransactionId);
                if (payment == null)
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Payment transaction not found"
                    };
                }

                // Verify payment with provider (mock implementation - integrate with actual UPI providers)
                var verificationResult = await VerifyWithProviderAsync(verifyPaymentDto.PaymentProvider, verifyPaymentDto.TransactionId);

                if (!verificationResult.IsSuccess)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = verificationResult.ErrorMessage;
                    await _paymentRepository.UpdateAsync(payment);

                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = verificationResult.ErrorMessage ?? "Payment verification failed"
                    };
                }

                // Update payment status
                payment.Status = PaymentStatus.Success;
                payment.PaymentDate = DateTime.UtcNow;
                payment.RazorpayOrderId = verificationResult.ProviderTransactionId;
                await _paymentRepository.UpdateAsync(payment);

                // Create user subscription
                var userSubscription = await CreateUserSubscriptionAsync(payment);

                return new PaymentVerificationResultDto
                {
                    IsSuccess = true,
                    Message = "Payment verified successfully",
                    UserSubscription = _mapper.Map<UserSubscriptionDto>(userSubscription),
                    PaymentTransaction = _mapper.Map<PaymentTransactionDto>(new PaymentTransaction())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying UPI payment: {TransactionId}", verifyPaymentDto.TransactionId);
                throw;
            }
        }

        public async Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto)
        {
            try
            {
                _logger.LogInformation("Processing refund for payment: {PaymentId}, amount: {Amount}", 
                    refundRequestDto.PaymentId, refundRequestDto.Amount);

                // Get payment
                var payment = await _paymentRepository.GetByIdAsync(refundRequestDto.PaymentId);
                if (payment == null)
                {
                    return new RefundResponseDto
                    {
                        IsSuccess = false,
                        Message = "Payment not found"
                    };
                }

                // Process refund with provider (mock implementation)
                var refundResult = await ProcessRefundWithProviderAsync(Enum.Parse<PaymentProvider>(payment.PaymentProvider), payment.RazorpayOrderId, refundRequestDto.Amount);

                if (!refundResult.IsSuccess)
                {
                    return new RefundResponseDto
                    {
                        IsSuccess = false,
                        Message = refundResult.ErrorMessage ?? "Refund processing failed"
                    };
                }

                // Update payment record
                payment.RefundAmount = refundRequestDto.Amount;
                payment.RefundDate = DateTime.UtcNow;
                payment.RefundReason = refundRequestDto.Reason;
                await _paymentRepository.UpdateAsync(payment);

                // Cancel user subscription if full refund
                if (payment.UserSubscriptionId.HasValue)
                {
                    var userSubscription = await _userSubscriptionRepository.GetByIdAsync(payment.UserSubscriptionId.Value);
                    if (userSubscription != null)
                    {
                        userSubscription.Status = "Cancelled";
                        await _userSubscriptionRepository.UpdateAsync(userSubscription);
                    }
                }

                return new RefundResponseDto
                {
                    IsSuccess = true,
                    RefundId = refundResult.RefundId,
                    RefundAmount = refundRequestDto.Amount,
                    Status = "Processed",
                    Message = "Refund processed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", refundRequestDto.PaymentId);
                throw;
            }
        }

        public async Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            try
            {
                var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
                return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment: {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentDto>> GetUserPaymentHistoryAsync(int userId)
        {
            try
            {
                var payments = await _paymentRepository.GetByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<PaymentDto>>(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<SubscriptionStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                return await _paymentRepository.GetStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment statistics");
                throw;
            }
        }

        public async Task<(IEnumerable<PaymentDto> Payments, int TotalCount)> GetPagedPaymentsAsync(AdminPaymentListRequestDto request)
        {
            try
            {
                var (payments, totalCount) = await _paymentRepository.GetPagedAsync(request.PageNumber, 20, null, null, request.Status, request.PaymentMethod);
                return (_mapper.Map<IEnumerable<PaymentDto>>(payments), totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged payments");
                throw;
            }
        }

        private Task<string> GenerateProviderOrderIdAsync(PaymentProvider provider, int paymentId)
        {
            var result = provider switch
            {
                PaymentProvider.GooglePay => $"GP_{paymentId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                PaymentProvider.Paytm => $"PT_{paymentId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                PaymentProvider.PhonePe => $"PP_{paymentId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                _ => $"UP_{paymentId}_{DateTime.UtcNow:yyyyMMddHHmmss}"
            };
            return Task.FromResult(result);
        }

        private Task<(string PaymentUrl, string QrCode)> GeneratePaymentDetailsAsync(PaymentProvider provider, string orderId, decimal amount, string currency)
        {
            // Mock implementation - integrate with actual UPI providers
            var baseUrl = _configuration["PaymentSettings:BaseUrl"] ?? "https://payment.example.com";
            var paymentUrl = $"{baseUrl}/pay/{orderId}";
            var qrCode = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"upi://pay?pa=merchant@upi&pn=RankUp&am={amount}&cu={currency}&tn={orderId}"));

            return Task.FromResult((paymentUrl, qrCode));
        }

        private async Task<ProviderVerificationResult> VerifyWithProviderAsync(PaymentProvider provider, string transactionId)
        {
            // Mock implementation - integrate with actual UPI providers
            await Task.Delay(100); // Simulate API call

            return new ProviderVerificationResult
            {
                IsSuccess = true,
                ProviderTransactionId = transactionId,
                ErrorMessage = null
            };
        }

        private async Task<ProviderRefundResult> ProcessRefundWithProviderAsync(PaymentProvider provider, string transactionId, decimal amount)
        {
            // Mock implementation - integrate with actual UPI providers
            await Task.Delay(100); // Simulate API call

            return new ProviderRefundResult
            {
                IsSuccess = true,
                RefundId = $"REF_{transactionId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                ErrorMessage = null
            };
        }

        private async Task<UserSubscription> CreateUserSubscriptionAsync(Payment payment)
        {
            // Get subscription plan
            var plan = await _subscriptionPlanRepository.GetByIdAsync(payment.SubscriptionPlanId);
            if (plan == null)
            {
                throw new KeyNotFoundException($"Subscription plan with ID {payment.SubscriptionPlanId} not found");
            }

            // Create user subscription
            var userSubscription = new UserSubscription
            {
                UserId = payment.UserId,
                SubscriptionPlanId = payment.SubscriptionPlanId,
                PaymentId = payment.Id,
                PurchasedDate = DateTime.UtcNow,
                ValidTill = DateTime.UtcNow.AddDays(plan.ValidityDays),
                TestsUsed = 0,
                TestsTotal = plan.TestPapersCount,
                AmountPaid = payment.FinalAmount,
                Currency = payment.Currency,
                DiscountApplied = payment.DiscountAmount,
                Status = "Active",
                AutoRenewal = false
            };

            var createdSubscription = await _userSubscriptionRepository.AddAsync(userSubscription);

            // Link payment -> subscription only after subscription record has actual ID.
            payment.UserSubscriptionId = createdSubscription.Id;
            await _paymentRepository.UpdateAsync(payment);

            return createdSubscription;
        }
    }

    // Helper classes for provider integration
    internal class ProviderVerificationResult
    {
        public bool IsSuccess { get; set; }
        public string ProviderTransactionId { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    internal class ProviderRefundResult
    {
        public bool IsSuccess { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
