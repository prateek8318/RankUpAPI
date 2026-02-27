using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly HomeDashboardDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(HomeDashboardDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entityName = typeof(T).Name;
            var parameters = new[] { new SqlParameter("@Id", id) };
            
            return await _context.Set<T>()
                .FromSqlRaw($"EXEC {entityName}_GetById @Id", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entityName = typeof(T).Name;
            
            return await _context.Set<T>()
                .FromSqlRaw($"EXEC {entityName}_GetAll")
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            // For complex predicates, create a specific stored procedure
            // This is a fallback for simple cases - consider creating SPs for complex queries
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var entityName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "CreatedAt" && p.Name != "UpdatedAt")
                .ToList();
            
            var parameterNames = properties.Select(p => $"@{p.Name}").ToArray();
            var propertyNames = properties.Select(p => p.Name).ToArray();
            
            var parameters = properties.Select(p => 
            {
                var value = p.GetValue(entity);
                return new SqlParameter($"@{p.Name}", value ?? DBNull.Value);
            }).ToArray();
            
            var sql = $"EXEC {entityName}_Insert {string.Join(", ", parameterNames)}";
            
            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var entityName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "CreatedAt")
                .ToList();
            
            var parameterNames = properties.Select(p => $"@{p.Name}").ToArray();
            
            var parameters = properties.Select(p => 
            {
                var value = p.GetValue(entity);
                return new SqlParameter($"@{p.Name}", value ?? DBNull.Value);
            }).ToArray();
            
            var sql = $"EXEC {entityName}_Update {string.Join(", ", parameterNames)}";
            
            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
            return await Task.FromResult(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entityName = typeof(T).Name;
            var parameters = new[] { new SqlParameter("@Id", id) };
            
            await _context.Database.ExecuteSqlRawAsync($"EXEC {entityName}_Delete @Id", parameters);
            return true;
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
