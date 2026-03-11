using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class GenericRepository<T> : ISubscriptionRepository<T> where T : BaseEntity
    {
        protected readonly SubscriptionDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(SubscriptionDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
