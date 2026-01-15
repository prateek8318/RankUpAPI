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
        private readonly IRazorpayService _razorpayService;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IRazorpayService razorpayService,
            IUserSubscriptionRepository userSubscriptionRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _razorpayService = razorpayService;
            _userSubscriptionRepository = userSubscriptionRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(CreateRazorpayOrderDto createOrderDto)
        {
            try
            {
                _logger.LogInformation("Creating Razorpay order for user: {UserId}, plan: {PlanId}", 
                    createOrderDto.UserId, createOrderDto.PlanId);

                // Get subscription plan
                var plan = await _subscriptionPlanRepository.GetByIdAsync(createOrderDto.PlanId);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {createOrderDto.PlanId} not found");
                }

                // Calculate final amount (no coupons)
                decimal originalAmount = plan.Price;
                decimal finalAmount = originalAmount;

                // Create Razorpay order
                var receipt = $"order_{createOrderDto.UserId}_{plan.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                var razorpayOrder = await _razorpayService.CreateOrderAsync(finalAmount, createOrderDto.Currency, receipt);

                // Create pending payment transaction
                var paymentTransaction = new PaymentTransaction
                {
                    UserSubscriptionId = 0, // Will be set when subscription is created
                    TransactionId = $"txn_{DateTime.UtcNow:yyyyMMddHHmmss}_{createOrderDto.UserId}",
                    RazorpayOrderId = razorpayOrder.Id,
                    Amount = finalAmount,
                    Currency = createOrderDto.Currency,
                    Status = PaymentStatus.Pending
                };

                await _paymentTransactionRepository.AddAsync(paymentTransaction);
                await _paymentTransactionRepository.SaveChangesAsync();

                var result = new RazorpayOrderResponseDto
                {
                    OrderId = razorpayOrder.Id,
                    Amount = finalAmount,
                    Currency = razorpayOrder.Currency,
                    Receipt = razorpayOrder.Receipt,
                    Status = razorpayOrder.Status,
                    CreatedAt = razorpayOrder.CreatedAt,
                    Key = _configuration["Razorpay:KeyId"] ?? string.Empty
                };

                _logger.LogInformation("Successfully created Razorpay order: {OrderId}", result.OrderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order for user: {UserId}", createOrderDto.UserId);
                throw;
            }
        }

        public async Task<PaymentVerificationResultDto> VerifyPaymentAsync(VerifyPaymentDto verifyPaymentDto)
        {
            try
            {
                _logger.LogInformation("Verifying payment for order: {OrderId}", verifyPaymentDto.RazorpayOrderId);

                // Verify payment with Razorpay
                var isPaymentValid = await _razorpayService.VerifyPaymentAsync(
                    verifyPaymentDto.RazorpayOrderId,
                    verifyPaymentDto.RazorpayPaymentId,
                    verifyPaymentDto.RazorpaySignature);

                if (!isPaymentValid)
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Payment verification failed"
                    };
                }

                // Get payment transaction
                var paymentTransaction = await _paymentTransactionRepository.GetByRazorpayOrderIdAsync(verifyPaymentDto.RazorpayOrderId);
                if (paymentTransaction == null)
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Payment transaction not found"
                    };
                }

                // Get payment details from Razorpay
                var paymentDetails = await _razorpayService.GetPaymentDetailsAsync(verifyPaymentDto.RazorpayPaymentId);

                // Update payment transaction
                paymentTransaction.RazorpayPaymentId = verifyPaymentDto.RazorpayPaymentId;
                paymentTransaction.RazorpaySignature = verifyPaymentDto.RazorpaySignature;
                paymentTransaction.Status = PaymentStatus.Completed;
                paymentTransaction.Method = GetPaymentMethodFromRazorpay(paymentDetails.Method);
                paymentTransaction.GatewayResponse = Newtonsoft.Json.JsonConvert.SerializeObject(paymentDetails);
                paymentTransaction.CompletedAt = DateTime.UtcNow;

                await _paymentTransactionRepository.UpdateAsync(paymentTransaction);
                await _paymentTransactionRepository.SaveChangesAsync();

                var result = new PaymentVerificationResultDto
                {
                    IsSuccess = true,
                    Message = "Payment verified successfully",
                    PaymentTransaction = _mapper.Map<PaymentTransactionDto>(paymentTransaction)
                };

                _logger.LogInformation("Successfully verified payment: {PaymentId}", verifyPaymentDto.RazorpayPaymentId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment: {PaymentId}", verifyPaymentDto.RazorpayPaymentId);
                throw;
            }
        }

        public async Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto)
        {
            try
            {
                _logger.LogInformation("Processing refund for payment: {PaymentId}, amount: {Amount}", 
                    refundRequestDto.PaymentId, refundRequestDto.Amount);

                // Get payment transaction
                var paymentTransaction = await _paymentTransactionRepository.GetByRazorpayPaymentIdAsync(refundRequestDto.PaymentId);
                if (paymentTransaction == null)
                {
                    return new RefundResponseDto
                    {
                        IsSuccess = false,
                        Message = "Payment transaction not found"
                    };
                }

                // Process refund with Razorpay
                var razorpayRefund = await _razorpayService.ProcessRefundAsync(refundRequestDto.PaymentId, refundRequestDto.Amount);

                // Update payment transaction
                if (razorpayRefund.Status.ToLower() == "processed")
                {
                    paymentTransaction.Status = PaymentStatus.Refunded;
                    paymentTransaction.RefundAmount = refundRequestDto.Amount;
                    paymentTransaction.RefundId = razorpayRefund.RefundId;
                    paymentTransaction.RefundedAt = DateTime.UtcNow;
                }
                else
                {
                    paymentTransaction.Status = PaymentStatus.PartiallyRefunded;
                    paymentTransaction.RefundAmount += refundRequestDto.Amount;
                    paymentTransaction.RefundId = razorpayRefund.RefundId;
                }

                await _paymentTransactionRepository.UpdateAsync(paymentTransaction);
                await _paymentTransactionRepository.SaveChangesAsync();

                var result = new RefundResponseDto
                {
                    IsSuccess = razorpayRefund.Status.ToLower() == "processed",
                    RefundId = razorpayRefund.RefundId,
                    RefundAmount = refundRequestDto.Amount,
                    Status = razorpayRefund.Status,
                    Message = razorpayRefund.Status.ToLower() == "processed" ? "Refund processed successfully" : "Refund processing"
                };

                _logger.LogInformation("Successfully processed refund: {RefundId}", result.RefundId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", refundRequestDto.PaymentId);
                throw;
            }
        }

        public async Task<PaymentTransactionDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            try
            {
                var paymentTransaction = await _paymentTransactionRepository.GetByTransactionIdAsync(transactionId);
                return paymentTransaction != null ? _mapper.Map<PaymentTransactionDto>(paymentTransaction) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment transaction: {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentTransactionDto>> GetUserPaymentHistoryAsync(int userId)
        {
            try
            {
                var paymentTransactions = await _paymentTransactionRepository.GetByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<PaymentTransactionDto>>(paymentTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user: {UserId}", userId);
                throw;
            }
        }

        private PaymentMethod GetPaymentMethodFromRazorpay(string razorpayMethod)
        {
            return razorpayMethod.ToLower() switch
            {
                "upi" => PaymentMethod.UPI,
                "card" => PaymentMethod.Card,
                "netbanking" => PaymentMethod.NetBanking,
                "wallet" => PaymentMethod.Wallet,
                "emi" => PaymentMethod.Card,
                _ => PaymentMethod.Card
            };
        }
    }
}
