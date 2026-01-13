using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;
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
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.UserId == userId);
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
            return await _context.Admins.ToListAsync();
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
