using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;
using TestService.Domain.Enums;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class UserTestAttemptRepository : GenericRepository<UserTestAttempt>, IUserTestAttemptRepository
    {
        public UserTestAttemptRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserTestAttempt>> GetOngoingByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.PracticeMode)
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.Series)
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.Subject)
                .Where(uta => uta.UserId == userId && uta.Status == TestAttemptStatus.InProgress)
                .ToListAsync();
        }

        public async Task<UserTestAttempt?> GetByIdWithTestAsync(int id)
        {
            return await _dbSet
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.PracticeMode)
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.Series)
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.Subject)
                .FirstOrDefaultAsync(uta => uta.Id == id);
        }

        public async Task<IEnumerable<UserTestAttempt>> GetByUserIdAsync(int userId, int limit = 10)
        {
            return await _dbSet
                .Include(uta => uta.Test)
                    .ThenInclude(t => t.PracticeMode)
                .Where(uta => uta.UserId == userId)
                .OrderByDescending(uta => uta.StartedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
