using MasterService.Domain.Entities;
using Common.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<PaginatedResponse<Category>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<Category>> GetActiveAsync();
        Task<PaginatedResponse<Category>> GetActiveAsync(PaginationRequest pagination);
        Task<IEnumerable<Category>> GetActiveByTypeAsync(string type);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ToggleCategoryStatusAsync(int id, bool isActive);
        Task<int> SaveChangesAsync();
    }
}

