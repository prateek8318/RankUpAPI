using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetDailyActiveUsersCountAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<int> SaveChangesAsync();
    }
}
