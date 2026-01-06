using RankUpAPI.Models;

namespace RankUpAPI.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(string phoneNumber);
        Task UpdateUserLoginInfoAsync(User user);
    }
}
