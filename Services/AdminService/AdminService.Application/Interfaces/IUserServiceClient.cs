using UserService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 50);
        Task<UserDto?> UpdateUserAsync(int id, object updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> EnableDisableUserAsync(int id, bool isActive);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetDailyActiveUsersCountAsync();
    }
}
