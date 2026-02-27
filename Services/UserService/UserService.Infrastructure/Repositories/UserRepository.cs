using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Application.DTOs;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE Id = @Id AND IsActive = 1";
                command.CommandType = CommandType.Text;
                
                var parameter = new SqlParameter("@Id", id);
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = new User();
                    user.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    user.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name"));
                    user.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"));
                    user.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                    user.CountryCode = reader.IsDBNull(reader.GetOrdinal("CountryCode")) ? null : reader.GetString(reader.GetOrdinal("CountryCode"));
                    user.Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : reader.GetString(reader.GetOrdinal("Gender"));
                    user.DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                    user.Qualification = reader.IsDBNull(reader.GetOrdinal("Qualification")) ? null : reader.GetString(reader.GetOrdinal("Qualification"));
                    user.PreferredLanguage = reader.IsDBNull(reader.GetOrdinal("PreferredLanguage")) ? null : reader.GetString(reader.GetOrdinal("PreferredLanguage"));
                    user.ProfilePhoto = reader.IsDBNull(reader.GetOrdinal("ProfilePhoto")) ? null : reader.GetString(reader.GetOrdinal("ProfilePhoto"));
                    user.PreferredExam = reader.IsDBNull(reader.GetOrdinal("PreferredExam")) ? null : reader.GetString(reader.GetOrdinal("PreferredExam"));
                    user.StateId = reader.IsDBNull(reader.GetOrdinal("StateId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("StateId"));
                    user.LanguageId = reader.IsDBNull(reader.GetOrdinal("LanguageId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("LanguageId"));
                    user.QualificationId = reader.IsDBNull(reader.GetOrdinal("QualificationId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("QualificationId"));
                    user.ExamId = reader.IsDBNull(reader.GetOrdinal("ExamId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ExamId"));
                    user.CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("CategoryId"));
                    user.StreamId = reader.IsDBNull(reader.GetOrdinal("StreamId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("StreamId"));
                    user.RefreshToken = reader.IsDBNull(reader.GetOrdinal("RefreshToken")) ? null : reader.GetString(reader.GetOrdinal("RefreshToken"));
                    user.RefreshTokenExpiryTime = reader.IsDBNull(reader.GetOrdinal("RefreshTokenExpiryTime")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiryTime"));
                    user.LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastLoginAt"));
                    user.IsPhoneVerified = reader.GetBoolean(reader.GetOrdinal("IsPhoneVerified"));
                    user.InterestedInIntlExam = reader.GetBoolean(reader.GetOrdinal("InterestedInIntlExam"));
                    user.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                    user.UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("UpdatedAt"));
                    user.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                    
                    return user;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {Id}", id);
                throw;
            }
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PhoneNumber", phoneNumber)
                };

                return await _context.Users
                    .FromSqlRaw("EXEC [dbo].[User_GetByPhoneNumber] @PhoneNumber", parameters)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by phone number: {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Email", email)
                };

                return await _context.Users
                    .FromSqlRaw("EXEC [dbo].[User_GetByEmail] @Email", parameters)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Page", page),
                    new SqlParameter("@PageSize", pageSize)
                };

                var users = await _context.Users
                    .FromSqlRaw("EXEC [dbo].[User_GetAll] @Page, @PageSize", parameters)
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection() as SqlConnection;
                if (connection != null && connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "EXEC [dbo].[User_GetTotalCount]";
                command.CommandType = CommandType.StoredProcedure;

                var count = await command.ExecuteScalarAsync();
                return Convert.ToInt32(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total user count");
                throw;
            }
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection() as SqlConnection;
                if (connection != null && connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "EXEC [dbo].[User_GetDailyActiveCount]";
                command.CommandType = CommandType.StoredProcedure;

                var count = await command.ExecuteScalarAsync();
                return Convert.ToInt32(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily active user count");
                throw;
            }
        }

        public async Task<User> AddAsync(User user)
        {
            try
            {
                var now = DateTime.UtcNow;

                using var connection = _context.Database.GetDbConnection() as SqlConnection;
                if (connection != null && connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection!.CreateCommand();
                command.CommandText = "[dbo].[User_Create]";
                command.CommandType = CommandType.StoredProcedure;

                var userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                
                command.Parameters.Add(new SqlParameter("@Name", (object?)user.Name ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Email", (object?)user.Email ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PasswordHash", (object?)user.PasswordHash ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ProfileImageUrl", (object?)user.ProfilePhoto ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@EmailVerified", SqlDbType.Bit) { Value = false });
                command.Parameters.Add(new SqlParameter("@IsActive", user.IsActive));
                command.Parameters.Add(new SqlParameter("@PreferredLanguage", (object?)user.PreferredLanguage ?? "en"));
                command.Parameters.Add(new SqlParameter("@CreatedAt", now));
                command.Parameters.Add(new SqlParameter("@UpdatedAt", now));
                command.Parameters.Add(userIdParam);

                await command.ExecuteNonQueryAsync();
                
                if (userIdParam.Value != DBNull.Value)
                {
                    user.Id = Convert.ToInt32(userIdParam.Value);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                var now = DateTime.UtcNow;

                // Ensure UpdatedAt has a value
                if (user.UpdatedAt == default)
                {
                    user.UpdatedAt = now;
                }

                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "[dbo].[User_Update]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@Id", user.Id));
                command.Parameters.Add(new SqlParameter("@Name", (object?)user.Name ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Email", (object?)user.Email ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CountryCode", (object?)user.CountryCode ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Gender", (object?)user.Gender ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@DateOfBirth", (object?)user.DateOfBirth ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Qualification", (object?)user.Qualification ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PreferredLanguage", (object?)user.PreferredLanguage ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ProfilePhoto", (object?)user.ProfilePhoto ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PreferredExam", (object?)user.PreferredExam ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@StateId", (object?)user.StateId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@LanguageId", (object?)user.LanguageId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@QualificationId", (object?)user.QualificationId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ExamId", (object?)user.ExamId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CategoryId", (object?)user.CategoryId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@StreamId", (object?)user.StreamId ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@RefreshToken", (object?)user.RefreshToken ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@RefreshTokenExpiryTime", (object?)user.RefreshTokenExpiryTime ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@IsPhoneVerified", user.IsPhoneVerified));
                command.Parameters.Add(new SqlParameter("@InterestedInIntlExam", user.InterestedInIntlExam));
                command.Parameters.Add(new SqlParameter("@IsActive", user.IsActive));
                command.Parameters.Add(new SqlParameter("@UpdatedAt", user.UpdatedAt));
                command.Parameters.Add(new SqlParameter("@LastLoginAt", (object?)user.LastLoginAt ?? DBNull.Value));

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with id: {Id}", user.Id);
                throw;
            }
        }

        public async Task DeleteAsync(User user)
        {
            try
            {
                var parameter = new SqlParameter("@UserId", user.Id);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[User_Delete] @UserId", 
                    parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id: {Id}", user.Id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists with id: {Id}", id);
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
