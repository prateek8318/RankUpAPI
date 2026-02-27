using Microsoft.Data.SqlClient;
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
        public DbSet<UserSocialLogin> UserSocialLogins { get; set; }

        // No OnModelCreating - using stored procedures instead
    }
}
