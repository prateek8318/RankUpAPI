using RankUpAPI.DTOs;
using RankUpAPI.Models;

namespace RankUpAPI.Services.Interfaces
{
    public interface IHomeContentService
    {
        Task<HomeSectionItemDto> CreateAsync(HomeSectionItemCreateDto dto, CancellationToken cancellationToken = default);
        Task<HomeSectionItemDto?> UpdateAsync(HomeSectionItemUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IList<HomeSectionItemDto>> GetBySectionAsync(HomeSectionType sectionType, int? examId, CancellationToken cancellationToken = default);
        Task<HomeContentResponseDto> GetHomeContentAsync(int? examId, CancellationToken cancellationToken = default);
    }
}

