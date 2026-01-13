using AdminService.Domain.Entities;

namespace AdminService.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByIdWithPermissionsAsync(int id);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task<int> SaveChangesAsync();
    }
}
