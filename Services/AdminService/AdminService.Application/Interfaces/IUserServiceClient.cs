using UserService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
    }
}
