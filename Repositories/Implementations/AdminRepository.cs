using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User != null && a.User.Email == email);
        }
    }
}
