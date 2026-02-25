using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _context.UserSocialLogins
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Provider == provider && s.GoogleId == googleId);
        }

        public async Task<UserSocialLogin?> GetByUserIdAndProviderAsync(int userId, string provider)
        {
            return await _context.UserSocialLogins
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Provider == provider);
        }

        public async Task<UserSocialLogin> AddAsync(UserSocialLogin socialLogin)
        {
            await _context.UserSocialLogins.AddAsync(socialLogin);
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

