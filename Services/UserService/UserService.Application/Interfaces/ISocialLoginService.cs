using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface ISocialLoginService
    {
        Task<SocialLoginResponseDto> SocialLoginAsync(MinimalSocialLoginRequestDto request);
        Task<SocialLoginResponseDto> LinkSocialAccountAsync(int userId, SocialLoginRequestDto request);
        Task<bool> UnlinkSocialAccountAsync(int userId, string provider);
        Task<bool> ValidateSocialTokenAsync(string provider, string accessToken);
    }

    public interface ISocialLoginRepository
    {
        Task<UserSocialLogin> GetByProviderAndGoogleIdAsync(string provider, string googleId);
        Task<UserSocialLogin> GetByUserIdAndProviderAsync(int userId, string provider);
        Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin);
        Task<UserSocialLogin> UpdateAsync(UserSocialLogin socialLogin);
        Task DeleteAsync(UserSocialLogin socialLogin);
        Task SaveChangesAsync();
    }
}
