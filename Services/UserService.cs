using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RankUpAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUserAsync(string phoneNumber)
        {
            try
            {
                var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);

                if (user == null)
                {
                    user = new User
                    {
                        PhoneNumber = phoneNumber,
                        Name = $"User{phoneNumber}",
                        // Do not auto-generate email; leave null so profile shows null until user updates it
                        Email = null,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                    await _userRepository.SaveChangesAsync();
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetOrCreateUserAsync] Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUserLoginInfoAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation($"Updated login info for user ID: {user.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating login info for user ID: {user?.Id}");
                throw;
            }
        }

        public async Task<User> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                // Update only the fields that are provided in the request
                if (!string.IsNullOrWhiteSpace(profileUpdate.Name))
                    user.Name = profileUpdate.Name;

                if (!string.IsNullOrWhiteSpace(profileUpdate.Email))
                    user.Email = profileUpdate.Email;

                if (!string.IsNullOrWhiteSpace(profileUpdate.Gender))
                    user.Gender = profileUpdate.Gender;

                if (profileUpdate.DateOfBirth.HasValue)
                    user.DateOfBirth = profileUpdate.DateOfBirth.Value;

                if (!string.IsNullOrWhiteSpace(profileUpdate.Qualification))
                    user.Qualification = profileUpdate.Qualification;

                if (!string.IsNullOrWhiteSpace(profileUpdate.LanguagePreference))
                    user.LanguagePreference = profileUpdate.LanguagePreference;

                if (!string.IsNullOrWhiteSpace(profileUpdate.PreferredExam))
                    user.PreferredExam = profileUpdate.PreferredExam;

                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation($"Updated profile for user ID: {userId}");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile for user ID: {userId}");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID: {userId}");
                throw;
            }
        }
    }
}