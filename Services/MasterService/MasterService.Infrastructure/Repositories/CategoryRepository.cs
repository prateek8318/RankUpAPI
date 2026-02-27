using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MasterDbContext _context;

        public CategoryRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            
            return await _context.Categories
                .FromSqlRaw("EXEC [dbo].[Category_GetById] @Id", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .FromSqlRaw("EXEC [dbo].[Category_GetAll]")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            return await _context.Categories
                .FromSqlRaw("EXEC [dbo].[Category_GetActive]")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveByTypeAsync(string type)
        {
            var parameters = new[] { new SqlParameter("@Type", type) };
            
            return await _context.Categories
                .FromSqlRaw("EXEC [dbo].[Category_GetActiveByType] @Type", parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            return category;
        }

        public Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

