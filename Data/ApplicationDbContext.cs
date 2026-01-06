using Microsoft.EntityFrameworkCore;
using RankUpAPI.Models;

namespace RankUpAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithOne(u => u.Admin)
                      .HasForeignKey<Admin>(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
