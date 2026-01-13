using AdminService.Domain.Entities;

namespace AdminService.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByIdAsync(int id);
        Task<Admin?> GetByUserIdAsync(int userId);
        Task<Admin?> GetByIdWithRolesAsync(int id);
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin> AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task<int> SaveChangesAsync();
    }
}
