using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto> GetOrCreateUserAsync(string phoneNumber);
        Task UpdateUserLoginInfoAsync(int userId);
        Task<UserDto> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate);
    }
}
