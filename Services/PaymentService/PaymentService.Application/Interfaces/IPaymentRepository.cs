using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByUserIdAsync(int userId);
        Task<Payment> AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<int> SaveChangesAsync();
    }
}
