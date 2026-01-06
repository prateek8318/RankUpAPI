using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RankUpAPI.Data;
using RankUpAPI.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RankUpAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUserAsync(string phoneNumber)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

                if (user == null)
                {
                    user = new User
                    {
                        PhoneNumber = phoneNumber,
                        Name = $"User{phoneNumber}",
                        Email = $"{phoneNumber}@rankup.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
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
                if (!_context.Users.Local.Any(u => u.Id == user.Id))
                {
                    _context.Users.Attach(user);
                }

                user.UpdatedAt = DateTime.UtcNow;

                // Prefer to update only UpdatedAt and IsPhoneVerified first to avoid issues with computed LastLoginAt column
                _context.Entry(user).Property(u => u.UpdatedAt).IsModified = true;
                _context.Entry(user).Property(u => u.IsPhoneVerified).IsModified = true;

                // Try to save; if it fails (e.g., computed column), attempt a fallback without LastLoginAt
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Updated login info for user ID: {user.Id}");
                    return;
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogWarning(dbEx, $"Initial save failed for user ID: {user.Id}, retrying without LastLoginAt");

                    // Remove attempts to modify LastLoginAt and UpdatedAt was already set
                    if (_context.Entry(user).Property(u => u.LastLoginAt) != null)
                    {
                        _context.Entry(user).Property(u => u.LastLoginAt).IsModified = false;
                    }

                    // As a safe fallback, do not touch LastLoginAt and only update UpdatedAt and IsPhoneVerified
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Fallback update succeeded for user ID: {user.Id}");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating login info for user ID: {user?.Id}");
                throw;
            }
        }
    }
}