using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AdminDbContext _context;

        public PermissionRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            return permission;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
