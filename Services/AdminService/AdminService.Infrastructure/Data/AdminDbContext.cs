using AdminService.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ExportLog> ExportLogs { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        // No OnModelCreating - using stored procedures instead
    }
}
