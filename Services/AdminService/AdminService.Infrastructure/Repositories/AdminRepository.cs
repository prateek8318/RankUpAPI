using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AdminDbContext _context;

        public AdminRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            
            return await _context.Admins
                .FromSqlRaw("EXEC [dbo].[Admin_GetById] @Id", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            var parameters = new[] { new SqlParameter("@UserId", userId) };
            
            return await _context.Admins
                .FromSqlRaw("EXEC [dbo].[Admin_GetByUserId] @UserId", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetByIdWithRolesAsync(int id)
        {
            return await _context.Admins
                .Include(a => a.AdminRoles)
                    .ThenInclude(ar => ar.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .FromSqlRaw("EXEC [dbo].[Admin_GetAll]")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Admin> AddAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            return admin;
        }

        public Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
