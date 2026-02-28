using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class SocialLoginDapperRepository : ISocialLoginRepository
    {
        private readonly string _connectionString;

        public SocialLoginDapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UserServiceConnection")!;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<UserSocialLogin?> GetByProviderAndGoogleIdAsync(string provider, string googleId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[UserSocialLogin_GetByProviderAndGoogleId] @Provider, @GoogleId";
            return await connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { Provider = provider, GoogleId = googleId });
        }

        public async Task<UserSocialLogin?> GetByUserIdAndProviderAsync(int userId, string provider)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[UserSocialLogin_GetByUserIdAndProvider] @UserId, @Provider";
            return await connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { UserId = userId, Provider = provider });
        }

        public async Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

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

            await connection.ExecuteAsync(sql, parameters);
            return socialLogin;
        }

        public Task<UserSocialLogin> UpdateAsync(UserSocialLogin socialLogin)
        {
            return Task.FromResult(socialLogin);
        }

        public Task DeleteAsync(UserSocialLogin socialLogin)
        {
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            // Dapper uses direct connections, no EF context needed
            await Task.CompletedTask;
        }
    }
}