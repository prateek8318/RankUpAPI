using QualificationService.Application.Interfaces;
using QualificationService.Domain.Entities;
using QualificationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace QualificationService.Infrastructure.Repositories
{
    public class StreamRepository : IStreamRepository
    {
        private readonly QualificationDbContext _context;

        public StreamRepository(QualificationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Stream?> GetByIdAsync(int id)
        {
            return await _context.Streams.FindAsync(id);
        }

        public async Task<IEnumerable<Domain.Entities.Stream>> GetAllAsync()
        {
            return await _context.Streams.ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Stream>> GetActiveAsync()
        {
            return await _context.Streams
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Stream> AddAsync(Domain.Entities.Stream stream)
        {
            await _context.Streams.AddAsync(stream);
            return stream;
        }

        public Task UpdateAsync(Domain.Entities.Stream stream)
        {
            _context.Streams.Update(stream);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Domain.Entities.Stream stream)
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
