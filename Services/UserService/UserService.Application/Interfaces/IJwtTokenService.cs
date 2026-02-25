using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        Task<bool> ValidateTokenAsync(string token);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string token);
    }
}
