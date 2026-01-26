using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class ExamRepository : GenericRepository<ExamMaster>, IExamRepository
    {
        public ExamRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExamMaster>> GetActiveExamsAsync()
        {
            return await _dbSet
                .Include(e => e.Subjects)
                .Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ExamMaster?> GetByIdWithSubjectsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Subjects)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
