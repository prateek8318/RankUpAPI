using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
                entity.HasIndex(u => u.PhoneNumber).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });
        }
    }
}
