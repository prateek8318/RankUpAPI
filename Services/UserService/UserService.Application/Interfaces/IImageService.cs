using Microsoft.AspNetCore.Http;

namespace UserService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadProfilePhotoAsync(IFormFile? imageFile, int userId);
        Task<bool> DeleteProfilePhotoAsync(string? imagePath);
    }
}
