using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto> GetOrCreateUserAsync(string phoneNumber, string? countryCode = null);
        Task UpdateUserLoginInfoAsync(int userId);
        Task<UserDto> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate);
        Task<UserDto> PatchUserProfileAsync(int userId, PatchProfileRequest patchRequest);
        Task<UserDto> PatchProfileWithImageAsync(int userId, PatchProfileFormData formData);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 50);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetDailyActiveUsersCountAsync();
    }
}
