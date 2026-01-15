using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto);
        Task<QuizDto?> UpdateQuizAsync(int id, UpdateQuizDto updateQuizDto);
        Task<bool> DeleteQuizAsync(int id);
        Task<bool> EnableDisableQuizAsync(int id, bool isActive);
        Task<QuizDto?> GetQuizByIdAsync(int id);
        Task<IEnumerable<QuizDto>> GetQuizzesByChapterIdAsync(int chapterId);
    }
}
