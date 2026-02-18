using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MasterService.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly MasterDbContext _context;
        private readonly ILogger<CountryRepository> _logger;

        public CountryRepository(MasterDbContext context, ILogger<CountryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            try
            {
                return await _context.Countries
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all countries");
                throw;
            }
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Countries
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country by id {CountryId}", id);
                throw;
            }
        }

        public async Task<Country?> GetByCodeAsync(string code)
        {
            try
            {
                return await _context.Countries
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country by code {CountryCode}", code);
                throw;
            }
        }

        public async Task<Country> AddAsync(Country country)
        {
            try
            {
                await _context.Countries.AddAsync(country);
                return country;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding country");
                throw;
            }
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            try
            {
                _context.Countries.Update(country);
                return country;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                throw;
            }
        }

        public async Task DeleteAsync(Country country)
        {
            try
            {
                _context.Countries.Remove(country);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }
    }
}
