using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Infrastructure.Data
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(s => s.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
