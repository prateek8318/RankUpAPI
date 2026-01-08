using RankUpAPI.Models;

namespace RankUpAPI.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(string phoneNumber);
        Task UpdateUserLoginInfoAsync(User user);
        Task<User> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
