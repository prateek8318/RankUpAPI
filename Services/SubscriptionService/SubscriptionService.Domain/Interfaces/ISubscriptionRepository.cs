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
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync();
        Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType);

        /// <summary>
        /// Prevent duplicate plan creation by English name (SubscriptionPlan.Name) within same exam category + type.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null);
    }

    public interface IUserSubscriptionRepository : ISubscriptionRepository<UserSubscription>
    {
        Task<UserSubscription?> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserSubscription>> GetByUserIdWithHistoryAsync(int userId);
        Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
        Task<UserSubscription?> GetByRazorpayOrderIdAsync(string orderId);
        Task<UserSubscription?> GetByRazorpayPaymentIdAsync(string paymentId);
    }

    public interface IPaymentTransactionRepository : ISubscriptionRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByTransactionIdAsync(string transactionId);
        Task<PaymentTransaction?> GetByRazorpayPaymentIdAsync(string razorpayPaymentId);
        Task<PaymentTransaction?> GetByRazorpayOrderIdAsync(string razorpayOrderId);
        Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(int userId);
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
