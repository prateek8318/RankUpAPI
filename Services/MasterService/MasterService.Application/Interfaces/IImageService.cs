using Microsoft.AspNetCore.Http;

namespace MasterService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadExamImageAsync(IFormFile? imageFile, int examId);
        Task<bool> DeleteExamImageAsync(string? imagePath);
    }
}
