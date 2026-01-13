using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByUserIdAsync(int userId);
        Task<Admin?> GetByEmailAsync(string email);
    }
}
