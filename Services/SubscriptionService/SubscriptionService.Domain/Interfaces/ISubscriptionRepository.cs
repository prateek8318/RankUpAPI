using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Domain.Interfaces
{
    public interface ISubscriptionRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }

    public interface ISubscriptionPlanRepository : ISubscriptionRepository<SubscriptionPlan>
    {
        Task<IEnumerable<SubscriptionPlan>> GetByExamCategoryAsync(string examCategory);
        Task<IEnumerable<SubscriptionPlan>> GetByExamIdAsync(int examId);
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync();
        Task<(IEnumerable<SubscriptionPlan> Plans, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, bool includeInactive, int? examId = null);
        Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType);

        /// <summary>
        /// Prevent duplicate plan creation by name within same exam (ExamId or ExamCategory) + type.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null, int? examId = null);
        
        // New methods for duration options support
        Task<SubscriptionPlan?> GetPlanWithDurationsAsync(int id, string languageCode = "en");
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansWithDurationsAsync(string languageCode = "en", int? examId = null);
        Task<IEnumerable<SubscriptionPlan>> GetAllPlansWithDurationsAsync(string languageCode = "en", int? examId = null);
        Task<PlanDurationOption?> GetDurationOptionAsync(int durationOptionId);
        Task AddDurationOptionAsync(PlanDurationOption durationOption);
        
        // Stats calculation methods
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<UserSubscription>> GetNewSubscriptionsAsync(DateTime startDate, DateTime endDate);
        Task UpdateDurationOptionAsync(PlanDurationOption durationOption);
        Task<UserSubscription?> GetUserActiveSubscriptionAsync(int userId, int planId);
        Task<IEnumerable<UserSubscription>> GetUserActiveSubscriptionsAsync(int userId);
        Task AddTranslationAsync(SubscriptionPlanTranslation translation);
        Task<SubscriptionPlan> CreatePlanWithDurationsAsync(SubscriptionPlan plan, IEnumerable<PlanDurationOption> durationOptions, IEnumerable<SubscriptionPlanTranslation>? translations = null);
    }

    public interface IUserSubscriptionRepository : ISubscriptionRepository<UserSubscription>
    {
        Task<UserSubscription?> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserSubscription>> GetByUserIdWithHistoryAsync(int userId);
        Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
        Task<IEnumerable<UserSubscription>> GetByPaymentIdAsync(int paymentId);
        Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(int userId);
        Task<(IEnumerable<UserSubscription> Subscriptions, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? userId = null, string? planName = null, SubscriptionStatus? status = null, bool? isActive = null, DateTime? startDateFrom = null, DateTime? startDateTo = null, DateTime? endDateFrom = null, DateTime? endDateTo = null, bool? autoRenew = null);
    }

    public interface IPaymentRepository : ISubscriptionRepository<Payment>
    {
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<Payment?> GetByProviderOrderIdAsync(string providerOrderId);
        Task<IEnumerable<Payment>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
        Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? userId = null, string? transactionId = null, PaymentStatus? status = null, PaymentMethod? paymentMethod = null, decimal? amountFrom = null, decimal? amountTo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null, string? providerOrderId = null);
        Task<(int TotalPayments, decimal TotalRevenue, decimal TodayRevenue, decimal ThisMonthRevenue, int SuccessfulPayments, int FailedPayments, int PendingPayments, decimal AverageTransactionAmount, int UniquePayingUsers)> GetStatisticsAsync();
    }

    public interface IInvoiceRepository : ISubscriptionRepository<Invoice>
    {
        Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<IEnumerable<Invoice>> GetByUserIdAsync(int userId);
        Task<Invoice?> GetByUserSubscriptionIdAsync(int userSubscriptionId);
        Task<string> GenerateInvoiceNumberAsync();
    }

    public interface IDemoAccessLogRepository : ISubscriptionRepository<DemoAccessLog>
    {
        Task<DemoAccessLog?> GetLastDemoAccessAsync(int userId, string examCategory);
        Task<IEnumerable<DemoAccessLog>> GetByUserIdAsync(int userId);
        Task<bool> HasUsedDemoAccessAsync(int userId, string examCategory);
    }
}
