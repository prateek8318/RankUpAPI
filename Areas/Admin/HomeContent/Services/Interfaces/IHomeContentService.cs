using RankUpAPI.Areas.Admin.HomeContent.DTOs;
using RankUpAPI.Models;

namespace RankUpAPI.Areas.Admin.HomeContent.Services.Interfaces
{
    public interface IHomeContentService
    {
        Task<HomeSectionItemDto> CreateAsync(HomeSectionItemCreateDto dto, CancellationToken cancellationToken = default);
        Task<HomeSectionItemDto?> UpdateAsync(HomeSectionItemUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IList<HomeSectionItemDto>> GetBySectionAsync(HomeSectionType sectionType, int? examId = null, CancellationToken cancellationToken = default);
    }
}
