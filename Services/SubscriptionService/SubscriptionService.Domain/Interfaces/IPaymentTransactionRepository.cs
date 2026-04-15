using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Domain.Interfaces
{
    public interface IPaymentTransactionRepository : ISubscriptionRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByTransactionIdAsync(string transactionId);
        Task<PaymentTransaction?> GetByProviderTransactionIdAsync(string providerTransactionId);
        Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PaymentTransaction>> GetByPaymentIdAsync(int paymentId);
    }
}
