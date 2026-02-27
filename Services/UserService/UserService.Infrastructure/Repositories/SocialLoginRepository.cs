using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class SocialLoginRepository : ISocialLoginRepository
    {
        private readonly UserDbContext _context;

        public SocialLoginRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<UserSocialLogin?> GetByProviderAndGoogleIdAsync(string provider, string googleId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Provider", provider),
                new SqlParameter("@GoogleId", googleId)
            };

            return await _context.UserSocialLogins
                .FromSqlRaw("EXEC [dbo].[UserSocialLogin_GetByProviderAndGoogleId] @Provider, @GoogleId", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserSocialLogin?> GetByUserIdAndProviderAsync(int userId, string provider)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Provider", provider)
            };

            return await _context.UserSocialLogins
                .FromSqlRaw("EXEC [dbo].[UserSocialLogin_GetByUserIdAndProvider] @UserId, @Provider", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", socialLogin.UserId),
                new SqlParameter("@Provider", socialLogin.Provider),
                new SqlParameter("@GoogleId", socialLogin.GoogleId),
                new SqlParameter("@AvatarUrl", (object?)socialLogin.AvatarUrl ?? DBNull.Value),
                new SqlParameter("@Email", socialLogin.Email),
                new SqlParameter("@Name", socialLogin.Name)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[UserSocialLogin_Insert] @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name", 
                parameters);
            
            return socialLogin;
        }

        public Task<UserSocialLogin> UpdateAsync(UserSocialLogin socialLogin)
        {
            _context.UserSocialLogins.Update(socialLogin);
            return Task.FromResult(socialLogin);
        }

        public Task DeleteAsync(UserSocialLogin socialLogin)
        {
            _context.UserSocialLogins.Remove(socialLogin);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

