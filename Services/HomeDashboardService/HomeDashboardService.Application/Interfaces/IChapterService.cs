using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IChapterService
    {
        Task<ChapterDto> CreateChapterAsync(CreateChapterDto createChapterDto);
        Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto updateChapterDto);
        Task<bool> DeleteChapterAsync(int id);
        Task<bool> EnableDisableChapterAsync(int id, bool isActive);
        Task<ChapterDto?> GetChapterByIdAsync(int id);
        Task<IEnumerable<ChapterDto>> GetChaptersBySubjectIdAsync(int subjectId);
    }
}
