using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class HomeSectionItemRepository : Repository<HomeSectionItem>, IHomeSectionItemRepository
    {
        public HomeSectionItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HomeSectionItem>> GetBySectionTypeAsync(HomeSectionType sectionType, int? examId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.SectionType == sectionType && x.IsVisible);

            if (examId.HasValue)
            {
                query = query.Where(x => x.ExamId == examId || x.ExamId == null);
            }

            return await query
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<HomeSectionItem>> GetVisibleAsync(int? examId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.IsVisible);

            if (examId.HasValue)
            {
                query = query.Where(x => x.ExamId == examId || x.ExamId == null);
            }

            return await query
                .OrderBy(x => x.SectionType)
                .ThenBy(x => x.DisplayOrder)
                .ToListAsync();
        }
    }
}
