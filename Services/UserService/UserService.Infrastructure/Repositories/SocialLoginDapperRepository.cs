using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class SocialLoginDapperRepository : ISocialLoginRepository
    {
        private readonly IDbConnection _connection;

        public SocialLoginDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<UserSocialLogin> GetByProviderAndGoogleIdAsync(string provider, string googleId)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_GetByProviderAndGoogleId] @Provider, @GoogleId";
            var result = await _connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { Provider = provider, GoogleId = googleId });
            return result != null && result.Id > 0 ? result : null;
        }

        public async Task<UserSocialLogin> GetByUserIdAndProviderAsync(int userId, string provider)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_GetByUserIdAndProvider] @UserId, @Provider";
            var result = await _connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { UserId = userId, Provider = provider });
            return result != null && result.Id > 0 ? result : null;
        }

        public async Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin)
        {
            var sql = @"EXEC [dbo].[UserSocialLogin_Insert] 
                    @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name, 
                    @AccessToken, @RefreshToken, @ExpiresAt";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", socialLogin.UserId);
            parameters.Add("@Provider", socialLogin.Provider);
            parameters.Add("@GoogleId", socialLogin.GoogleId);
            parameters.Add("@AvatarUrl", socialLogin.AvatarUrl);
            parameters.Add("@Email", socialLogin.Email);
            parameters.Add("@Name", socialLogin.Name);
            parameters.Add("@AccessToken", socialLogin.AccessToken);
            parameters.Add("@RefreshToken", socialLogin.RefreshToken);
            parameters.Add("@ExpiresAt", socialLogin.ExpiresAt);

            await _connection.ExecuteAsync(sql, parameters);
            return socialLogin;
        }

        public async Task<UserSocialLogin> UpdateAsync(UserSocialLogin socialLogin)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_Update] @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name, @AccessToken, @RefreshToken, @ExpiresAt";
            
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", socialLogin.UserId);
            parameters.Add("@Provider", socialLogin.Provider);
            parameters.Add("@GoogleId", socialLogin.GoogleId);
            parameters.Add("@AvatarUrl", socialLogin.AvatarUrl);
            parameters.Add("@Email", socialLogin.Email);
            parameters.Add("@Name", socialLogin.Name);
            parameters.Add("@AccessToken", socialLogin.AccessToken);
            parameters.Add("@RefreshToken", socialLogin.RefreshToken);
            parameters.Add("@ExpiresAt", socialLogin.ExpiresAt);

            await _connection.ExecuteAsync(sql, parameters);
            return socialLogin;
        }

        public async Task DeleteAsync(UserSocialLogin socialLogin)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_Delete] @UserId, @Provider";
            await _connection.ExecuteAsync(sql, new { UserId = socialLogin.UserId, Provider = socialLogin.Provider });
        }

        public async Task SaveChangesAsync()
        {
            // Dapper uses direct connections, no EF context needed
            await Task.CompletedTask;
        }
    }
}