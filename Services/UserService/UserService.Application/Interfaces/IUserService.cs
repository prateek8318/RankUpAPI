using UserService.Application.DTOs;



namespace UserService.Application.Interfaces

{

    public interface IUserService

    {

        Task<UserDto?> GetUserByIdAsync(int userId);

        Task<UserDto> GetOrCreateUserAsync(string phoneNumber, string? countryCode = null, bool markPhoneVerified = false);

        Task UpdateUserLoginInfoAsync(int userId);
        
        Task UpdateUserDeviceInfoAsync(int userId, string? deviceId = null, string? deviceType = null, string? deviceName = null);

        Task<UserDto> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate);

        Task<UserDto> PatchUserProfileAsync(int userId, PatchProfileRequest patchRequest);

        Task<UserDto> PatchProfileWithImageAsync(int userId, PatchProfileFormData formData);

        Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 50);

        Task<int> GetTotalUsersCountAsync();

        Task<int> GetDailyActiveUsersCountAsync();

    }

}

