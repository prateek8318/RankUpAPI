using AdminService.Domain.Entities;

namespace AdminService.Application.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(int id);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> AddAsync(Permission permission);
        Task<int> SaveChangesAsync();
    }
}
