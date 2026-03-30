using PaymentService.Domain.Entities;
using Common.DTOs;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<PaginatedResponse<Payment>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<Payment>> GetByUserIdAsync(int userId);
        Task<PaginatedResponse<Payment>> GetByUserIdAsync(int userId, PaginationRequest pagination);
        Task<Payment> AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<int> SaveChangesAsync();
    }
}
