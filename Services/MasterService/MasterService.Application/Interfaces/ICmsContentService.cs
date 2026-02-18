using MasterService.Application.DTOs;
using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface ICmsContentService
    {
        // User side: language ke basis par localized data (Hindi/English with fallback)
        Task<CmsContentDto?> GetByKeyAsync(string key, string language);
        Task<IEnumerable<CmsContentDto>> GetAllAsync(string language);
        IReadOnlyList<string> GetAllowedKeys();

        // Admin side: create/update/delete
        Task<CmsContentDto> CreateAsync(CreateCmsContentDto createDto);
        Task<CmsContentDto?> UpdateAsync(int id, UpdateCmsContentDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, CmsContentStatus status);
    }
}

