using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Infrastructure.Data
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<SubscriptionPlanTranslation> SubscriptionPlanTranslations { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<DemoAccessLog> DemoAccessLogs { get; set; }

        // Keep the old Subscription for backward compatibility
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => (c1 ?? new()).SequenceEqual(c2 ?? new()),
                c => (c ?? new()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (c ?? new()).ToList());

            // Configure SubscriptionPlan
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.ExamCategory).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.DurationType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CardColorTheme).HasMaxLength(50);
                entity.Property(e => e.Features)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(stringListComparer);

                entity.HasIndex(e => e.ExamCategory);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsActive);
            });

            modelBuilder.Entity<SubscriptionPlanTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Features)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(stringListComparer);

                entity.HasIndex(e => new { e.SubscriptionPlanId, e.LanguageCode }).IsUnique();

                entity.HasOne(e => e.SubscriptionPlan)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(e => e.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Match SubscriptionPlan's soft-delete filter
                entity.HasQueryFilter(t => t.SubscriptionPlan.IsActive);
            });


            // Configure UserSubscription
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RazorpayOrderId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RazorpayPaymentId).HasMaxLength(100);
                entity.Property(e => e.RazorpaySignature).HasMaxLength(100);
                entity.Property(e => e.OriginalAmount).HasPrecision(18, 2);
                entity.Property(e => e.FinalAmount).HasPrecision(18, 2);
                entity.Property(e => e.RazorpaySubscriptionId).HasMaxLength(100);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.SubscriptionPlanId);
                entity.HasIndex(e => e.RazorpayOrderId);
                entity.HasIndex(e => e.RazorpayPaymentId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.EndDate);

                // Foreign key relationships
                entity.HasOne(e => e.SubscriptionPlan)
                    .WithMany(e => e.UserSubscriptions)
                    .HasForeignKey(e => e.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RazorpayOrderId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RazorpayPaymentId).HasMaxLength(100);
                entity.Property(e => e.RazorpaySignature).HasMaxLength(100);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.GatewayResponse).HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                entity.Property(e => e.RefundId).HasMaxLength(100);

                entity.HasIndex(e => e.UserSubscriptionId);
                entity.HasIndex(e => e.TransactionId);
                entity.HasIndex(e => e.RazorpayOrderId);
                entity.HasIndex(e => e.RazorpayPaymentId);
                entity.HasIndex(e => e.Status);

                // Foreign key relationship
                entity.HasOne(e => e.UserSubscription)
                    .WithMany(e => e.PaymentTransactions)
                    .HasForeignKey(e => e.UserSubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.BillingAddress).HasMaxLength(500);
                entity.Property(e => e.CustomerEmail).HasMaxLength(500);
                entity.Property(e => e.CustomerName).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(2000);
                entity.Property(e => e.PdfFilePath).HasMaxLength(500);

                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.UserSubscriptionId);
                entity.HasIndex(e => e.Status);

                // Foreign key relationship
                entity.HasOne(e => e.UserSubscription)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey<Invoice>(e => e.UserSubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DemoAccessLog
            modelBuilder.Entity<DemoAccessLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExamCategory).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IPAddress).HasMaxLength(100);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.DeviceType).HasMaxLength(50);
                entity.Property(e => e.AccessDetails).HasMaxLength(1000);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExamCategory);
                entity.HasIndex(e => e.AccessDate);
            });

            // Configure global query filters for soft delete
            modelBuilder.Entity<SubscriptionPlan>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<UserSubscription>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<PaymentTransaction>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Invoice>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<DemoAccessLog>().HasQueryFilter(e => e.IsActive);

            // Keep the old Subscription configuration for backward compatibility
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(s => s.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
