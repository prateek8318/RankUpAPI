using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IAdminService
    {
        Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request);
        Task<AdminAuthResponse> VerifyOtpAsync(AdminOtpVerificationRequest request);
        Task<AdminDto?> GetAdminByIdAsync(int id);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
        Task LogActivityAsync(int adminId, string action, string? resource = null, int? resourceId = null, string? details = null);
    }
}
