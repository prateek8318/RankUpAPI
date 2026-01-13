using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
