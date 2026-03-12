using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class UserDapperRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[User_GetById] @Id";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var sql = "EXEC [dbo].[User_GetByPhoneNumber] @PhoneNumber";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { PhoneNumber = phoneNumber });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var sql = "EXEC [dbo].[User_GetByEmail] @Email";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            var sql = "EXEC [dbo].[User_GetAll] @Page, @PageSize";
            return await _connection.QueryAsync<User>(sql, new { Page = page, PageSize = pageSize });
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            var sql = "EXEC [dbo].[User_GetTotalCount]";
            return await _connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            var sql = "EXEC [dbo].[User_GetDailyActiveCount]";
            return await _connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<User> AddAsync(User user)
        {
            var sql = @"EXEC [dbo].[User_Create] 
                    @Name, @Email, @PhoneNumber, @PasswordHash, 
                    @ProfilePhoto, @IsActive, 
                    @PreferredLanguage, @CreatedAt, @UpdatedAt, @UserId OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", user.Name);
            parameters.Add("@Email", user.Email);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@ProfilePhoto", user.ProfilePhoto);
            parameters.Add("@IsActive", user.IsActive);
            parameters.Add("@PreferredLanguage", user.PreferredLanguage ?? "en");
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters);

            var userId = parameters.Get<int>("@UserId");
            if (userId > 0)
            {
                user.Id = userId;
            }

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            var sql = @"EXEC [dbo].[User_Update] 
                    @Id, @Name, @Email, @PhoneNumber, @CountryCode, @Gender,
                    @DateOfBirth, @Qualification, @PreferredLanguage, @ProfilePhoto,
                    @PreferredExam, @StateId, @LanguageId, @QualificationId,
                    @ExamId, @CategoryId, @StreamId, @RefreshToken,
                    @RefreshTokenExpiryTime, @IsPhoneVerified, @InterestedInIntlExam,
                    @IsActive, @UpdatedAt, @LastLoginAt";

            await _connection.ExecuteAsync(sql, user);
        }

        public async Task DeleteAsync(User user)
        {
            var sql = "EXEC [dbo].[User_Delete] @UserId";
            await _connection.ExecuteAsync(sql, new { UserId = user.Id });
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var user = await GetByIdAsync(id);
            return user != null;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper uses direct connections, no EF context needed
            return await Task.FromResult(1);
        }
    }
}