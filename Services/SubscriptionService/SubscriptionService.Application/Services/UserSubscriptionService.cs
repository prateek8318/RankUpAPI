using AutoMapper;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRazorpayService _razorpayService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserSubscriptionService> _logger;

        public UserSubscriptionService(
            IUserSubscriptionRepository userSubscriptionRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            IInvoiceRepository invoiceRepository,
            IRazorpayService razorpayService,
            IMapper mapper,
            ILogger<UserSubscriptionService> logger)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _invoiceRepository = invoiceRepository;
            _razorpayService = razorpayService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserSubscriptionDto> CreateSubscriptionAsync(CreateUserSubscriptionDto createSubscriptionDto)
        {
            try
            {
                _logger.LogInformation("Creating subscription for user: {UserId}, plan: {PlanId}", 
                    createSubscriptionDto.UserId, createSubscriptionDto.SubscriptionPlanId);

                // Get subscription plan
                var plan = await _subscriptionPlanRepository.GetByIdAsync(createSubscriptionDto.SubscriptionPlanId);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {createSubscriptionDto.SubscriptionPlanId} not found");
                }

                // Calculate amounts (no coupons)
                decimal originalAmount = plan.Price;
                decimal finalAmount = originalAmount;

                // Create subscription
                var subscription = new UserSubscription
                {
                    UserId = createSubscriptionDto.UserId,
                    SubscriptionPlanId = createSubscriptionDto.SubscriptionPlanId,
                    RazorpayOrderId = "", // Not available in CreateUserSubscriptionDto
                    AmountPaid = finalAmount,
                    DiscountApplied = originalAmount - finalAmount,
                    Status = "Pending",
                    AutoRenewal = createSubscriptionDto.AutoRenewal
                };

                var createdSubscription = await _userSubscriptionRepository.AddAsync(subscription);

                var result = _mapper.Map<UserSubscriptionDto>(createdSubscription);
                
                _logger.LogInformation("Successfully created subscription with ID: {SubscriptionId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user: {UserId}", createSubscriptionDto.UserId);
                throw;
            }
        }

        public async Task<PaymentVerificationResultDto> ActivateSubscriptionAsync(ActivateSubscriptionDto activateSubscriptionDto)
        {
            try
            {
                _logger.LogInformation("Activating subscription for order: {OrderId}", activateSubscriptionDto.RazorpayOrderId);

                // Verify payment with Razorpay
                var isPaymentValid = await _razorpayService.VerifyPaymentAsync(
                    activateSubscriptionDto.RazorpayOrderId,
                    activateSubscriptionDto.RazorpayPaymentId,
                    activateSubscriptionDto.RazorpaySignature);

                if (!isPaymentValid)
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Payment verification failed"
                    };
                }

                // Get subscription by order ID
                var subscriptions = await _userSubscriptionRepository.GetByUserIdWithHistoryAsync(activateSubscriptionDto.UserId);
                var subscription = subscriptions.FirstOrDefault(s => s.RazorpayOrderId == activateSubscriptionDto.RazorpayOrderId);
                if (subscription == null)
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Subscription not found for the given order"
                    };
                }

                // Get payment details from Razorpay
                var paymentDetails = await _razorpayService.GetPaymentDetailsAsync(activateSubscriptionDto.RazorpayPaymentId);

                // Create payment transaction
                var paymentTransaction = new PaymentTransaction
                {
                    UserSubscriptionId = subscription.Id,
                    TransactionId = $"txn_{DateTime.UtcNow:yyyyMMddHHmmss}_{subscription.Id}",
                    RazorpayOrderId = activateSubscriptionDto.RazorpayOrderId,
                    RazorpayPaymentId = activateSubscriptionDto.RazorpayPaymentId,
                    RazorpaySignature = activateSubscriptionDto.RazorpaySignature,
                    Amount = subscription.AmountPaid,
                    Currency = "INR",
                    Status = PaymentStatus.Success,
                    Method = GetPaymentMethodFromRazorpay(paymentDetails.Method),
                    GatewayResponse = Newtonsoft.Json.JsonConvert.SerializeObject(paymentDetails),
                    CompletedAt = DateTime.UtcNow
                };

                await _paymentTransactionRepository.AddAsync(paymentTransaction);

                // Activate subscription
                subscription.RazorpayPaymentId = activateSubscriptionDto.RazorpayPaymentId;
                subscription.RazorpaySignature = activateSubscriptionDto.RazorpaySignature;
                subscription.Status = "Active";
                subscription.PurchasedDate = DateTime.UtcNow;
                subscription.ValidTill = DateTime.UtcNow.AddDays(subscription.SubscriptionPlan.ValidityDays);
                subscription.UpdatedAt = DateTime.UtcNow;

                await _userSubscriptionRepository.UpdateAsync(subscription);

                // Generate invoice
                await GenerateInvoiceForSubscription(subscription);

                var result = new PaymentVerificationResultDto
                {
                    IsSuccess = true,
                    Message = "Payment verified and subscription activated successfully",
                    UserSubscription = _mapper.Map<UserSubscriptionDto>(subscription),
                    PaymentTransaction = _mapper.Map<PaymentTransactionDto>(paymentTransaction)
                };

                _logger.LogInformation("Successfully activated subscription: {SubscriptionId}", subscription.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription for order: {OrderId}", activateSubscriptionDto.RazorpayOrderId);
                throw;
            }
        }

        public async Task<UserSubscriptionDto> RenewSubscriptionAsync(RenewSubscriptionDto renewSubscriptionDto)
        {
            try
            {
                _logger.LogInformation("Renewing subscription: {SubscriptionId}", renewSubscriptionDto.SubscriptionId);

                var subscription = await _userSubscriptionRepository.GetByIdAsync(renewSubscriptionDto.SubscriptionId);
                if (subscription == null)
                {
                    throw new KeyNotFoundException($"Subscription with ID {renewSubscriptionDto.SubscriptionId} not found");
                }

                // Get the plan
                var plan = await _subscriptionPlanRepository.GetByIdAsync(subscription.SubscriptionPlanId);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {subscription.SubscriptionPlanId} not found");
                }

                // Calculate amounts for renewal (no coupons)
                decimal originalAmount = plan.Price;
                decimal finalAmount = originalAmount;

                // Create new subscription for renewal
                var newSubscription = new UserSubscription
                {
                    UserId = subscription.UserId,
                    SubscriptionPlanId = subscription.SubscriptionPlanId,
                    RazorpayOrderId = $"renewal_{DateTime.UtcNow:yyyyMMddHHmmss}_{subscription.Id}",
                    AmountPaid = finalAmount,
                    DiscountApplied = originalAmount - finalAmount,
                    Status = "Pending",
                    AutoRenewal = renewSubscriptionDto.AutoRenewal
                };

                var createdSubscription = await _userSubscriptionRepository.AddAsync(newSubscription);

                // Update old subscription
                subscription.Status = "Expired";
                subscription.UpdatedAt = DateTime.UtcNow;
                await _userSubscriptionRepository.UpdateAsync(subscription);

                var result = _mapper.Map<UserSubscriptionDto>(createdSubscription);
                
                _logger.LogInformation("Successfully created renewal subscription: {SubscriptionId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription: {SubscriptionId}", renewSubscriptionDto.SubscriptionId);
                throw;
            }
        }

        public async Task<bool> CancelSubscriptionAsync(CancelSubscriptionDto cancelSubscriptionDto)
        {
            try
            {
                _logger.LogInformation("Cancelling subscription: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);

                var subscription = await _userSubscriptionRepository.GetByIdAsync(cancelSubscriptionDto.SubscriptionId);
                if (subscription == null)
                {
                    throw new KeyNotFoundException($"Subscription with ID {cancelSubscriptionDto.SubscriptionId} not found");
                }

                subscription.Status = "Cancelled";
                // Note: CancelledDate and CancellationReason are not properties in UserSubscription entity
                // These would need to be added to the entity if needed
                subscription.UpdatedAt = DateTime.UtcNow;

                await _userSubscriptionRepository.UpdateAsync(subscription);

                _logger.LogInformation("Successfully cancelled subscription: {SubscriptionId}", subscription.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);
                throw;
            }
        }

        public async Task<UserSubscriptionDto?> GetSubscriptionByIdAsync(int id)
        {
            try
            {
                var subscription = await _userSubscriptionRepository.GetByIdAsync(id);
                return subscription != null ? _mapper.Map<UserSubscriptionDto>(subscription) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription with ID: {SubscriptionId}", id);
                throw;
            }
        }

        public async Task<UserSubscriptionDto?> GetMySubscriptionAsync(int userId)
        {
            try
            {
                var subscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
                return subscription != null ? _mapper.Map<UserSubscriptionDto>(subscription) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserSubscriptionDto?> GetActiveSubscriptionForUserAsync(int userId)
        {
            try
            {
                var subscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
                if (subscription == null)
                    return null;

                // Map the subscription with all computed fields from database
                var subscriptionDto = _mapper.Map<UserSubscriptionDto>(subscription);
                
                // The database query already includes SubscriptionPlan details, so no need for separate call
                // Just ensure the navigation property is properly mapped
                if (subscription.SubscriptionPlan != null)
                {
                    subscriptionDto.SubscriptionPlan = _mapper.Map<SubscriptionPlanDto>(subscription.SubscriptionPlan);
                }

                return subscriptionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<SubscriptionHistoryDto> GetUserSubscriptionHistoryAsync(int userId)
        {
            try
            {
                var subscriptions = await _userSubscriptionRepository.GetByUserIdWithHistoryAsync(userId);
                var subscriptionDtos = _mapper.Map<IEnumerable<UserSubscriptionDto>>(subscriptions);

                var history = new SubscriptionHistoryDto
                {
                    UserId = userId,
                    Subscriptions = subscriptionDtos.ToList(),
                    ActiveSubscriptionCount = subscriptionDtos.Count(s => s.Status == "Active"),
                    ExpiredSubscriptionCount = subscriptionDtos.Count(s => s.Status == "Expired"),
                    CancelledSubscriptionCount = subscriptionDtos.Count(s => s.Status == "Cancelled"),
                    TotalSpent = subscriptionDtos.Where(s => s.Status == "Active" || s.Status == "Expired")
                                              .Sum(s => s.AmountPaid)
                };

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription history for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserSubscriptionDto>> GetAllUserSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await _userSubscriptionRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user subscriptions");
                throw;
            }
        }

        public async Task<IEnumerable<UserSubscriptionDto>> GetActiveSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await _userSubscriptionRepository.GetActiveSubscriptionsAsync();
                return _mapper.Map<IEnumerable<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscriptions");
                throw;
            }
        }

        public async Task<IEnumerable<UserSubscriptionDto>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            try
            {
                var subscriptions = await _userSubscriptionRepository.GetExpiringSubscriptionsAsync(daysBeforeExpiry);
                return _mapper.Map<IEnumerable<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring subscriptions");
                throw;
            }
        }

        private async Task GenerateInvoiceForSubscription(UserSubscription subscription)
        {
            try
            {
                var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();
                
                var invoice = new Invoice
                {
                    UserSubscriptionId = subscription.Id,
                    InvoiceNumber = invoiceNumber,
                    InvoiceDate = DateTime.UtcNow,
                    Subtotal = subscription.AmountPaid,
                    TaxAmount = 0, // Calculate tax based on your requirements
                    TotalAmount = subscription.AmountPaid,
                    Currency = "INR",
                    Status = InvoiceStatus.Generated,
                    CustomerName = $"User {subscription.UserId}" // Get from user service if needed
                };

                await _invoiceRepository.AddAsync(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for subscription: {SubscriptionId}", subscription.Id);
                // Don't throw here as invoice generation failure shouldn't affect subscription activation
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

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null)
        {
            try
            {
                _logger.LogInformation("Getting active subscription plans for language: {Language}", language);
                
                var currentLanguage = language ?? "en";
                var plans = await _subscriptionPlanRepository.GetActivePlansAsync();
                var planDtos = _mapper.Map<List<SubscriptionPlanListDto>>(plans);
                
                // Apply localization if needed
                ApplyLocalization(planDtos, plans, currentLanguage);
                
                return planDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                throw;
            }
        }

        private static void ApplyLocalization(List<SubscriptionPlanListDto> dtos, IEnumerable<SubscriptionPlan> entities, string? language)
        {
            var lang = NormalizeLanguage(language);
            foreach (var dto in dtos)
            {
                dto.ExamType = dto.ExamCategory;
                if (lang == "en") continue;

                var entity = entities.FirstOrDefault(e => e.Id == dto.Id);
                if (entity == null) continue;

                var t = entity.Translations.FirstOrDefault(x => x.LanguageCode == lang);
                if (t == null) continue;

                dto.Name = t.Name;
                dto.Description = t.Description;
                dto.Features = t.Features ?? new List<string>();
            }
        }

        private static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language)) return "en";
            return language.Trim().ToLowerInvariant();
        }
    }
}
