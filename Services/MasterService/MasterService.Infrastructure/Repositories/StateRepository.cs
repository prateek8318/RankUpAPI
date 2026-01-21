using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly MasterDbContext _context;

        public StateRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<State?> GetByIdAsync(int id)
        {
            return await _context.States
                .Include(s => s.StateLanguages)
                .ThenInclude(sl => sl.Language)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.States
                .Include(s => s.StateLanguages)
                .ThenInclude(sl => sl.Language)
                .ToListAsync();
        }

        public async Task<IEnumerable<State>> GetActiveAsync()
        {
            return await _context.States
                .Include(s => s.StateLanguages)
                .ThenInclude(sl => sl.Language)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<State> AddAsync(State state)
        {
            await _context.States.AddAsync(state);
            return state;
        }

        public Task UpdateAsync(State state)
        {
            _context.States.Update(state);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(State state)
        {
            _context.States.Remove(state);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
