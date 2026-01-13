using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AdminDbContext _context;

        public RoleRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            return role;
        }

        public Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
