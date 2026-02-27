using Microsoft.Data.SqlClient;
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
            var parameters = new[] { new SqlParameter("@Id", id) };
            
            return await _context.Payments
                .FromSqlRaw("EXEC [dbo].[Payment_GetById] @Id", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            var parameters = new[] { new SqlParameter("@TransactionId", transactionId) };
            
            return await _context.Payments
                .FromSqlRaw("EXEC [dbo].[Payment_GetByTransactionId] @TransactionId", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .FromSqlRaw("EXEC [dbo].[Payment_GetAll]")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId)
        {
            var parameters = new[] { new SqlParameter("@UserId", userId) };
            
            return await _context.Payments
                .FromSqlRaw("EXEC [dbo].[Payment_GetByUserId] @UserId", parameters)
                .AsNoTracking()
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
