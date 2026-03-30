using UserService.Domain.Entities;
using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByIdAsync(int id);
        Task<User?> GetUserEntityByIdAsync(int id);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<User>> GetActiveAsync(int page = 1, int pageSize = 50);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetDailyActiveUsersCountAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<int> SaveChangesAsync();
    }
}
