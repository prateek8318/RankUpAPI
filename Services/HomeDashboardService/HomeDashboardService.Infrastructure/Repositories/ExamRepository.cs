using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public ExamRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Exam>> GetActiveExamsAsync()
        {
            return await _dbSet
                .Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ThenBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdWithSubjectsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Subjects.Where(s => s.IsActive))
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }
    }
}
