using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common; // Added missing using directive for DbCommand
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


        public async Task<UserSocialLogin?> GetByProviderAndGoogleIdAsync(string provider, string googleId)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_GetByProviderAndGoogleId] @Provider, @GoogleId";
            return await _connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { Provider = provider, GoogleId = googleId });
        }

        public async Task<UserSocialLogin?> GetByUserIdAndProviderAsync(int userId, string provider)
        {
            var sql = "EXEC [dbo].[UserSocialLogin_GetByUserIdAndProvider] @UserId, @Provider";
            return await _connection.QueryFirstOrDefaultAsync<UserSocialLogin>(sql, new { UserId = userId, Provider = provider });
        }

        public async Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin)
        {
            // Set proper QUOTED_IDENTIFIER setting for stored procedures
            using var setCommand = _connection.CreateCommand();
            setCommand.CommandText = "SET QUOTED_IDENTIFIER ON";
            await ((DbCommand)setCommand).ExecuteNonQueryAsync();

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