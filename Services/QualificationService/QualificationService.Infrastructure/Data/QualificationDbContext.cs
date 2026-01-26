using QualificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace QualificationService.Infrastructure.Data
{
    public class QualificationDbContext : DbContext
    {
        public QualificationDbContext(DbContextOptions<QualificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Domain.Entities.Stream> Streams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Stream entity
            modelBuilder.Entity<Domain.Entities.Stream>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Qualification entity
            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                // Configure relationship with Stream
                entity.HasOne(q => q.Stream)
                      .WithMany()
                      .HasForeignKey(q => q.StreamId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
