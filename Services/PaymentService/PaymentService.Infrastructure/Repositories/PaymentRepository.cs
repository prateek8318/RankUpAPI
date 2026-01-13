using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId && p.IsActive)
                .ToListAsync();
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return payment;
        }

        public Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
