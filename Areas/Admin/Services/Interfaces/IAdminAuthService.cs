using RankUpAPI.Areas.Admin.Models.Auth;

namespace RankUpAPI.Areas.Admin.Services.Interfaces
{
    public interface IAdminAuthService
    {
        Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request);
        Task<AdminAuthResponse> RefreshTokenAsync(string token, string refreshToken);
    }
}
