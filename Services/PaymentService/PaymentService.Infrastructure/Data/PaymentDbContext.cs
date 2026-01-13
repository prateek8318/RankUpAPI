using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(p => p.TransactionId).IsUnique();
                entity.Property(p => p.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
