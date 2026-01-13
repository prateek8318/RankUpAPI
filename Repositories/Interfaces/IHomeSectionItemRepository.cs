using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IHomeSectionItemRepository : IRepository<HomeSectionItem>
    {
        Task<IEnumerable<HomeSectionItem>> GetBySectionTypeAsync(HomeSectionType sectionType, int? examId = null);
        Task<IEnumerable<HomeSectionItem>> GetVisibleAsync(int? examId = null);
    }
}
