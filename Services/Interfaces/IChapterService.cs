using RankUpAPI.DTOs;

namespace RankUpAPI.Services.Interfaces
{
    public interface IChapterService
    {
        Task<ChapterDto> CreateChapterAsync(CreateChapterDto createDto);
        Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto updateDto);
        Task<bool> DeleteChapterAsync(int id);
        Task<ChapterDto?> GetChapterByIdAsync(int id);
        Task<IEnumerable<ChapterDto>> GetAllChaptersAsync();
        Task<IEnumerable<ChapterDto>> GetChaptersBySubjectIdAsync(int subjectId);
        Task<bool> ToggleChapterStatusAsync(int id, bool isActive);
    }
}
