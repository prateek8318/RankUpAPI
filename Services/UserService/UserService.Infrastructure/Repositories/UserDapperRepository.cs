using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class UserDapperRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserDapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UserServiceConnection")!;
        }

        private SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            return connection;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetByPhoneNumber] @PhoneNumber";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { PhoneNumber = phoneNumber });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetByEmail] @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetAll] @Page, @PageSize";
            return await connection.QueryAsync<User>(sql, new { Page = page, PageSize = pageSize });
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetTotalCount]";
            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_GetDailyActiveCount]";
            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<User> AddAsync(User user)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            // Set proper QUOTED_IDENTIFIER setting for stored procedures
            using var setCommand = connection.CreateCommand();
            setCommand.CommandText = "SET QUOTED_IDENTIFIER ON";
            await setCommand.ExecuteNonQueryAsync();

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

            await connection.ExecuteAsync(sql, parameters);

            var userId = parameters.Get<int>("@UserId");
            if (userId > 0)
            {
                user.Id = userId;
            }

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = @"EXEC [dbo].[User_Update] 
                    @Id, @Name, @Email, @PhoneNumber, @CountryCode, @Gender,
                    @DateOfBirth, @Qualification, @PreferredLanguage, @ProfilePhoto,
                    @PreferredExam, @StateId, @LanguageId, @QualificationId,
                    @ExamId, @CategoryId, @StreamId, @RefreshToken,
                    @RefreshTokenExpiryTime, @IsPhoneVerified, @InterestedInIntlExam,
                    @IsActive, @UpdatedAt, @LastLoginAt";

            await connection.ExecuteAsync(sql, user);
        }

        public async Task DeleteAsync(User user)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var sql = "EXEC [dbo].[User_Delete] @UserId";
            await connection.ExecuteAsync(sql, new { UserId = user.Id });
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