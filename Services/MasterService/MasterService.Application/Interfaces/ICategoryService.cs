using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(string language);
        Task<IEnumerable<CategoryDto>> GetQualificationsAsync(string language);
        Task<IEnumerable<CategoryDto>> GetExamCategoriesAsync(string language);
        Task<IEnumerable<CategoryDto>> GetStreamsAsync(string language);
        Task<object> GetAllCategoriesOptimizedAsync(string language);
        Task<CategoryDto> GetCategoryAsync(int id, string language);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> ToggleCategoryStatusAsync(int id, bool isActive);
    }
}
