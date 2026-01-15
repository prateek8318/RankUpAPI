using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Subject>> GetByExamIdAsync(int examId)
        {
            return await _dbSet
                .Where(s => s.ExamId == examId && s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdWithChaptersAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Chapters.Where(c => c.IsActive))
                .Include(s => s.Exam)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }
    }
}
