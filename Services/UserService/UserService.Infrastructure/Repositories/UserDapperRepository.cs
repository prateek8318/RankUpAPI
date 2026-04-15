using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class UserDapperRepository : BaseDapperRepository, IUserRepository
    {
        public UserDapperRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var sql = @"
SELECT
    u.Id,
    u.Name,
    u.Email,
    u.PhoneNumber,
    u.CountryCode,
    u.Gender,
    u.DateOfBirth,
    u.Qualification,
    u.PreferredLanguage AS LanguagePreference,
    u.ProfilePhoto,
    u.PreferredExam,
    u.StateId,
    s.Name AS StateName,
    CAST(NULL AS NVARCHAR(100)) AS StateNameHi,
    u.LanguageId,
    l.Name AS LanguageName,
    l.Name AS LanguageNameHi,
    u.QualificationId,
    q.Name AS QualificationName,
    q.NameHi AS QualificationNameHi,
    u.ExamId,
    CAST(NULL AS NVARCHAR(100)) AS ExamName,
    CAST(NULL AS NVARCHAR(100)) AS ExamNameHi,
    u.CategoryId,
    c.NameEn AS CategoryName,
    c.NameHi AS CategoryNameHi,
    u.StreamId,
    st.Name AS StreamName,
    st.NameHi AS StreamNameHi,
    u.LastLoginAt,
    u.IsPhoneVerified,
    u.IsActive,
    u.CreatedAt,
    u.ProfileCompleted,
    u.InterestedInIntlExam,
    u.DeviceId,
    u.DeviceType,
    u.DeviceName,
    u.FcmToken,
    u.LastDeviceLoginAt,
    u.LastDeviceType,
    u.LastDeviceName,
    u.LoginType,
    DATEADD(MINUTE, 330, u.CreatedAt) AS CreatedAtIST,
    CASE
        WHEN u.LastLoginAt IS NULL THEN NULL
        ELSE DATEADD(MINUTE, 330, u.LastLoginAt)
    END AS LastLoginAtIST
FROM [dbo].[Users] u
LEFT JOIN [RankUp_MasterDB].[dbo].[States] s ON s.Id = u.StateId
LEFT JOIN [RankUp_MasterDB].[dbo].[Languages] l ON l.Id = u.LanguageId
LEFT JOIN [RankUp_MasterDB].[dbo].[Qualifications] q ON q.Id = u.QualificationId
LEFT JOIN [RankUp_MasterDB].[dbo].[Categories] c ON c.Id = u.CategoryId
LEFT JOIN [RankUp_MasterDB].[dbo].[Streams] st ON st.Id = u.StreamId
WHERE u.Id = @Id
  AND u.IsActive = 1;";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = id }));
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[User_GetById_Basic] @Id";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id }));
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var sql = "EXEC [dbo].[User_GetByPhoneNumber] @PhoneNumber";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<User>(sql, new { PhoneNumber = phoneNumber }));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var sql = "EXEC [dbo].[User_GetByEmail] @Email";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email }));
        }

        public async Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            var sql = "EXEC [dbo].[User_GetAll] @Page, @PageSize";
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<User>(sql, new { Page = page, PageSize = pageSize }));
        }

        public async Task<IEnumerable<User>> GetActiveAsync(int page = 1, int pageSize = 50)
        {
            var sql = "EXEC [dbo].[User_GetActive] @Page, @PageSize";
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<User>(sql, new { Page = page, PageSize = pageSize }));
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            var sql = "EXEC [dbo].[User_GetTotalCount]";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<int>(sql));
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            var sql = "EXEC [dbo].[User_GetDailyActiveCount]";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<int>(sql, commandTimeout: 300));
        }

        public async Task<User> AddAsync(User user)
        {
            var sql = @"EXEC [dbo].[User_Create] 
                    @Name, @Email, @PhoneNumber, @PasswordHash, 
                    @ProfilePhoto, @IsActive, 
                    @PreferredLanguage, @IsPhoneVerified, @InterestedInIntlExam,
                    @CreatedAt, @UpdatedAt, @UserId OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", user.Name);
            parameters.Add("@Email", user.Email);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@ProfilePhoto", user.ProfilePhoto);
            parameters.Add("@IsActive", user.IsActive);
            parameters.Add("@PreferredLanguage", user.PreferredLanguage ?? "en");
            parameters.Add("@IsPhoneVerified", user.IsPhoneVerified);
            parameters.Add("@InterestedInIntlExam", user.InterestedInIntlExam);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, parameters));

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

            var parameters = new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CountryCode = user.CountryCode,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Qualification = user.Qualification,
                PreferredLanguage = user.PreferredLanguage,
                ProfilePhoto = user.ProfilePhoto,
                PreferredExam = user.PreferredExam,
                StateId = user.StateId,
                LanguageId = user.LanguageId,
                QualificationId = user.QualificationId,
                ExamId = user.ExamId,
                CategoryId = user.CategoryId,
                StreamId = user.StreamId,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                IsPhoneVerified = user.IsPhoneVerified,
                InterestedInIntlExam = user.InterestedInIntlExam,
                IsActive = user.IsActive,
                UpdatedAt = DateTime.UtcNow,
                LastLoginAt = user.LastLoginAt
            };

            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, parameters));
        }

        public async Task DeleteAsync(User user)
        {
            var sql = "EXEC [dbo].[User_Delete] @UserId";
            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, new { UserId = user.Id }));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var user = await GetUserEntityByIdAsync(id);
            return user != null;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper uses direct connections, no EF context needed
            return await Task.FromResult(0);
        }
    }
}
