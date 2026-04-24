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
                if (createSubscriptionDto.SubscriptionPlanId <= 0)
                {
                    throw new ArgumentException("Valid SubscriptionPlanId is required");
                }

                _logger.LogInformation("Creating subscription for user: {UserId}, plan: {PlanId}", 
                    createSubscriptionDto.UserId, createSubscriptionDto.SubscriptionPlanId);

                // Get subscription plan
                var plan = await _subscriptionPlanRepository.GetByIdAsync(createSubscriptionDto.SubscriptionPlanId);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {createSubscriptionDto.SubscriptionPlanId} not found");
                }

                // Get duration option if provided
                int validityDays = plan.ValidityDays;
                decimal finalAmount = plan.Price;
                
                PlanDurationOption durationOption = null;
                if (createSubscriptionDto.DurationOptionId > 0)
                {
                    durationOption = await _subscriptionPlanRepository.GetDurationOptionAsync(createSubscriptionDto.DurationOptionId);
                    if (durationOption != null)
                    {
                        validityDays = durationOption.DurationMonths * 30;
                        finalAmount = durationOption.Price;
                    }
                }

                // Create order at Razorpay before persisting subscription.
                // This ensures frontend gets a valid order id for payment flow.
                RazorpayOrderResponse razorpayOrder;
                try
                {
                    var receipt = $"sub_{createSubscriptionDto.UserId}_{createSubscriptionDto.SubscriptionPlanId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                    razorpayOrder = await _razorpayService.CreateOrderAsync(finalAmount, "INR", receipt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to create Razorpay order for user: {UserId}, plan: {PlanId}",
                        createSubscriptionDto.UserId, createSubscriptionDto.SubscriptionPlanId);
                    throw new InvalidOperationException("Unable to create Razorpay order. Please try again.");
                }

                var subscription = new UserSubscription
                {
                    UserId = createSubscriptionDto.UserId,
                    SubscriptionPlanId = createSubscriptionDto.SubscriptionPlanId,
                    DurationOptionId = createSubscriptionDto.DurationOptionId,
                    RazorpayOrderId = razorpayOrder.Id,
                    PurchasedDate = DateTime.UtcNow,
                    ValidTill = DateTime.UtcNow.AddDays(validityDays),
                    TestsUsed = 0,
                    TestsTotal = createSubscriptionDto.DurationOptionId > 0 && durationOption != null ? durationOption.TestPapersCount : plan.TestPapersCount,
                    AmountPaid = finalAmount,
                    Currency = "INR",
                    DiscountApplied = plan.Price > finalAmount ? plan.Price - finalAmount : 0,
                    Status = "Pending",
                    AutoRenewal = createSubscriptionDto.AutoRenewal,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdSubscription = await _userSubscriptionRepository.AddAsync(subscription);

                var result = _mapper.Map<UserSubscriptionDto>(createdSubscription);
                ApplyRealTimeFields(result);
                
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
                if (string.IsNullOrEmpty(activateSubscriptionDto.RazorpayPaymentId) || 
                    string.IsNullOrEmpty(activateSubscriptionDto.RazorpaySignature))
                {
                    return new PaymentVerificationResultDto
                    {
                        IsSuccess = false,
                        Message = "Payment ID and signature are required for payment verification"
                    };
                }

                var isPaymentValid = await _razorpayService.VerifyPaymentAsync(
                    activateSubscriptionDto.RazorpayOrderId,
                    activateSubscriptionDto.RazorpayPaymentId!,
                    activateSubscriptionDto.RazorpaySignature!);

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
                
                if (result.UserSubscription != null)
                {
                    ApplyRealTimeFields(result.UserSubscription);
                }

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
                ApplyRealTimeFields(result);
                
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
                if (subscription == null) return null;
                var dto = _mapper.Map<UserSubscriptionDto>(subscription);
                ApplyRealTimeFields(dto);
                return dto;
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
                if (subscription == null)
                    return null;

                if (subscription.SubscriptionPlan == null)
                {
                    subscription.SubscriptionPlan = await _subscriptionPlanRepository.GetByIdAsync(subscription.SubscriptionPlanId);
                }

                if (subscription.SubscriptionPlan != null)
                {
                    subscription.ExamName ??= subscription.SubscriptionPlan.Name;
                    subscription.ExamDescription ??= subscription.SubscriptionPlan.Description;
                    subscription.ExamImageUrl ??= subscription.SubscriptionPlan.ImageUrl;
                }

                var subscriptionDto = _mapper.Map<UserSubscriptionDto>(subscription);
                
                // Ensure SubscriptionPlan details are properly mapped
                if (subscription.SubscriptionPlan != null)
                {
                    subscriptionDto.SubscriptionPlan = _mapper.Map<SubscriptionPlanDto>(subscription.SubscriptionPlan);
                }

                // Format dates properly for API response
                subscriptionDto.PurchasedDate = DateTime.SpecifyKind(subscription.PurchasedDate, DateTimeKind.Utc);
                subscriptionDto.ValidTill = DateTime.SpecifyKind(subscription.ValidTill, DateTimeKind.Utc);

                ApplyRealTimeFields(subscriptionDto);
                
                // Ensure duration information is accurate
                if (subscriptionDto.SubscriptionPlan != null)
                {
                    subscriptionDto.SubscriptionPlan.DurationType = subscriptionDto.SubscriptionPlan.DurationType ?? "Monthly";
                }

                return subscriptionDto;
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

                if (subscription.SubscriptionPlan == null)
                {
                    subscription.SubscriptionPlan = await _subscriptionPlanRepository.GetByIdAsync(subscription.SubscriptionPlanId);
                }

                if (subscription.SubscriptionPlan != null)
                {
                    subscription.ExamName ??= subscription.SubscriptionPlan.Name;
                    subscription.ExamDescription ??= subscription.SubscriptionPlan.Description;
                    subscription.ExamImageUrl ??= subscription.SubscriptionPlan.ImageUrl;
                }

                // Map the subscription with all computed fields from database
                var subscriptionDto = _mapper.Map<UserSubscriptionDto>(subscription);
                
                // The database query already includes SubscriptionPlan details, so no need for separate call
                // Just ensure the navigation property is properly mapped
                if (subscription.SubscriptionPlan != null)
                {
                    subscriptionDto.SubscriptionPlan = _mapper.Map<SubscriptionPlanDto>(subscription.SubscriptionPlan);
                }

                ApplyRealTimeFields(subscriptionDto);
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
                var subscriptionDtos = _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
                foreach (var dto in subscriptionDtos)
                {
                    ApplyRealTimeFields(dto);
                }
                await EnrichSubscriptionDtosAsync(subscriptionDtos);

                var history = new SubscriptionHistoryDto
                {
                    UserId = userId,
                    Subscriptions = subscriptionDtos,
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
                var dtos = _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
                foreach (var dto in dtos)
                {
                    ApplyRealTimeFields(dto);
                }
                await EnrichSubscriptionDtosAsync(dtos);
                return dtos;
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
                var dtos = _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
                foreach (var dto in dtos)
                {
                    ApplyRealTimeFields(dto);
                }
                return dtos;
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
                var dtos = _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
                foreach (var dto in dtos)
                {
                    ApplyRealTimeFields(dto);
                }
                return dtos;
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

        private async Task EnrichSubscriptionDtosAsync(List<UserSubscriptionDto> dtos)
        {
            if (dtos.Count == 0)
            {
                return;
            }

            var planIds = dtos.Select(x => x.SubscriptionPlanId).Distinct().ToList();
            var planMap = new Dictionary<int, SubscriptionPlan>();

            foreach (var planId in planIds)
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (plan != null)
                {
                    planMap[planId] = plan;
                }
            }

            foreach (var dto in dtos)
            {
                if (!planMap.TryGetValue(dto.SubscriptionPlanId, out var plan))
                {
                    continue;
                }

                dto.SubscriptionName = plan.Name;
                dto.IsPopular = plan.IsPopular;
                dto.ExamName ??= plan.Name;
                dto.ExamDescription ??= plan.Description;
                dto.ExamImageUrl ??= plan.ImageUrl;

                if (dto.SubscriptionPlan == null)
                {
                    dto.SubscriptionPlan = _mapper.Map<SubscriptionPlanDto>(plan);
                }
            }
        }

        private static void ApplyRealTimeFields(UserSubscriptionDto dto)
        {
            // NOTE: Some DB queries may project these fields; always compute them at response-time.
            var now = DateTime.UtcNow;
            var daysRemaining = dto.ValidTill > now
                ? (int)Math.Ceiling((dto.ValidTill - now).TotalDays)
                : 0;

            dto.DaysLeft = daysRemaining;
            dto.DaysUntilExpiry = daysRemaining;

            dto.CurrentStatus =
                string.Equals(dto.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) ? "Cancelled" :
                dto.ValidTill <= now ? "Expired" :
                string.Equals(dto.Status, "Pending", StringComparison.OrdinalIgnoreCase) ? "Pending" :
                "Active";

            dto.IsExpired =
                !string.Equals(dto.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) &&
                dto.ValidTill <= now;
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null, int? examId = null)
        {
            try
            {
                _logger.LogInformation("Getting active subscription plans for language: {Language}", language);
                
                var currentLanguage = language ?? "en";
                var plans = await _subscriptionPlanRepository.GetActivePlansWithDurationsAsync(currentLanguage);
                var planDtos = _mapper.Map<List<SubscriptionPlanListDto>>(plans);

                // Filter by selected exam (user chooses during profile completion)
                // In this system ExamCategory often contains Master ExamId as string.
                if (examId.HasValue)
                {
                    planDtos = planDtos
                        .Where(p => string.Equals(p.ExamCategory, examId.Value.ToString(), StringComparison.OrdinalIgnoreCase)
                                 || (p.ExamId.HasValue && p.ExamId.Value == examId.Value))
                        .ToList();
                }
                
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

        public async Task<UserManagementStatsDto> GetUserManagementStatsAsync()
        {
            try
            {
                var stats = new UserManagementStatsDto
                {
                    LastUpdated = DateTime.UtcNow
                };

                // Get all users (this would typically come from UserService)
                // For now, we'll use subscription data to estimate
                var allSubscriptions = await _userSubscriptionRepository.GetAllAsync();
                var uniqueUserIds = allSubscriptions.Select(s => s.UserId).Distinct().ToList();
                
                stats.TotalUsers = uniqueUserIds.Count;
                
                // Get active subscribers
                var activeSubscriptions = allSubscriptions.Where(s => s.Status == "Active").ToList();
                stats.ActiveSubscribers = activeSubscriptions.Select(s => s.UserId).Distinct().Count();
                
                // Free users = Total users - Active subscribers
                stats.FreeUsers = stats.TotalUsers - stats.ActiveSubscribers;
                
                // New users in last 7 days (based on subscription creation)
                var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                var newUsers = allSubscriptions.Where(s => s.CreatedAt >= sevenDaysAgo)
                                            .Select(s => s.UserId)
                                            .Distinct()
                                            .Count();
                stats.NewUsersLast7Days = newUsers;
                
                // Blocked users (this would need integration with UserService)
                // For now, returning 0 as placeholder
                stats.BlockedUsers = 0;

                _logger.LogInformation("User management stats calculated: Total={Total}, Active={Active}, Free={Free}, New={New}",
                    stats.TotalUsers, stats.ActiveSubscribers, stats.FreeUsers, stats.NewUsersLast7Days);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating user management stats");
                throw;
            }
        }

        public async Task<IEnumerable<UserManagementDto>> GetAllUsersWithSubscriptionDetailsAsync()
        {
            try
            {
                var allSubscriptions = await _userSubscriptionRepository.GetAllAsync();
                var userManagementList = new List<UserManagementDto>();

                // Group subscriptions by user
                var userGroups = allSubscriptions.GroupBy(s => s.UserId);

                foreach (var userGroup in userGroups)
                {
                    var latestSubscription = userGroup.OrderByDescending(s => s.CreatedAt).First();
                    var activeSubscription = userGroup.FirstOrDefault(s => s.Status == "Active");
                    var latestStatus = string.Equals(latestSubscription.Status, "Cancelled", StringComparison.OrdinalIgnoreCase)
                        ? "Cancelled"
                        : latestSubscription.ValidTill < DateTime.UtcNow
                            ? "Expired"
                            : "Active";

                    var userManagement = new UserManagementDto
                    {
                        Id = userGroup.Key,
                        CreatedAt = latestSubscription.CreatedAt,
                        Status = latestStatus,
                        HasActiveSubscription = activeSubscription != null,
                        SubscriptionId = activeSubscription?.Id ?? latestSubscription.Id,
                        DaysRemaining = activeSubscription != null ? 
                            Math.Max(0, (int)Math.Ceiling((activeSubscription.ValidTill - DateTime.UtcNow).TotalDays)) : 0
                    };

                    // Add subscription details if active
                    if (activeSubscription != null)
                    {
                        userManagement.SubscriptionExpiryDate = activeSubscription.ValidTill;
                        userManagement.SubscriptionAmount = activeSubscription.AmountPaid;
                        
                        // Get plan details
                        var plan = await _subscriptionPlanRepository.GetByIdAsync(activeSubscription.SubscriptionPlanId);
                        if (plan != null)
                        {
                            userManagement.PlanName = plan.Name;
                        }
                    }
                    else
                    {
                        userManagement.SubscriptionExpiryDate = latestSubscription.ValidTill;
                        userManagement.SubscriptionAmount = latestSubscription.AmountPaid;

                        var latestPlan = await _subscriptionPlanRepository.GetByIdAsync(latestSubscription.SubscriptionPlanId);
                        if (latestPlan != null)
                        {
                            userManagement.PlanName = latestPlan.Name;
                        }
                    }

                    userManagementList.Add(userManagement);
                }

                _logger.LogInformation("Retrieved {Count} users with subscription details", userManagementList.Count);
                return userManagementList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users with subscription details");
                throw;
            }
        }

        public async Task<BlockUserResponseDto> BlockUserAsync(BlockUserDto blockUserDto)
        {
            try
            {
                // This would integrate with UserService to actually block/unblock the user
                // For now, we'll simulate the blocking by updating subscription status
                
                var userSubscriptions = await _userSubscriptionRepository.GetAllAsync();
                var userSubs = userSubscriptions.Where(s => s.UserId == blockUserDto.UserId).ToList();

                if (!userSubs.Any())
                {
                    return new BlockUserResponseDto
                    {
                        Success = false,
                        Message = "User not found or has no subscriptions",
                        UserId = blockUserDto.UserId,
                        NewStatus = "Unknown"
                    };
                }

                // If blocking, cancel all active subscriptions
                if (blockUserDto.Block)
                {
                    foreach (var subscription in userSubs.Where(s => s.Status == "Active"))
                    {
                        subscription.Status = "Cancelled";
                        subscription.UpdatedAt = DateTime.UtcNow;
                        await _userSubscriptionRepository.UpdateAsync(subscription);
                    }
                }

                var response = new BlockUserResponseDto
                {
                    Success = true,
                    Message = blockUserDto.Block ? 
                        $"User {blockUserDto.UserId} blocked successfully. Reason: {blockUserDto.Reason}" :
                        $"User {blockUserDto.UserId} unblocked successfully",
                    UserId = blockUserDto.UserId,
                    NewStatus = blockUserDto.Block ? "Blocked" : "Active"
                };

                _logger.LogInformation("User {UserId} status changed to {Status}", 
                    blockUserDto.UserId, response.NewStatus);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking/unblocking user {UserId}", blockUserDto.UserId);
                throw;
            }
        }
    }
}
