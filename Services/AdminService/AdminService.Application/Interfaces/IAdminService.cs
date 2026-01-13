using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IAdminService
    {
        Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request);
        Task<AdminDto?> GetAdminByIdAsync(int id);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
        Task LogActivityAsync(int adminId, string action, string? resource = null, int? resourceId = null, string? details = null);
    }
}
