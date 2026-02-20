using MasterService.Application.Interfaces;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Infrastructure.Repositories
{
    public class StreamRepository : IStreamRepository
    {
        private readonly MasterDbContext _context;

        public StreamRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<StreamEntity?> GetByIdAsync(int id)
        {
            return await _context.Streams
                .Include(s => s.Qualification)
                .Include(s => s.StreamLanguages)
                .ThenInclude(sl => sl.Language)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<StreamEntity>> GetAllAsync()
        {
            return await _context.Streams
                .Include(s => s.Qualification)
                .Include(s => s.StreamLanguages)
                .ThenInclude(sl => sl.Language)
                .ToListAsync();
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveAsync()
        {
            return await _context.Streams
                .Include(s => s.Qualification)
                .Include(s => s.StreamLanguages)
                .ThenInclude(sl => sl.Language)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId)
        {
            return await _context.Streams
                .Include(s => s.Qualification)
                .Include(s => s.StreamLanguages)
                .ThenInclude(sl => sl.Language)
                .Where(s => s.IsActive && s.QualificationId == qualificationId)
                .ToListAsync();
        }

        public async Task<StreamEntity> AddAsync(StreamEntity stream)
        {
            await _context.Streams.AddAsync(stream);
            return stream;
        }

        public Task UpdateAsync(StreamEntity stream)
        {
            _context.Streams.Update(stream);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(StreamEntity stream)
        {
            _context.Streams.Remove(stream);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
