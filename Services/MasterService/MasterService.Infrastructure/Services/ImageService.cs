using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MasterService.Application.Interfaces;

namespace MasterService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IHostEnvironment environment, ILogger<ImageService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string?> UploadExamImageAsync(IFormFile? imageFile, int examId)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            try
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Invalid file type: {FileExtension}", fileExtension);
                    throw new InvalidOperationException("Only image files (jpg, jpeg, png, gif, webp) are allowed.");
                }

                // Validate file size (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("File too large: {FileSize} bytes", imageFile.Length);
                    throw new InvalidOperationException("File size must be less than 5MB.");
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "exams");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"exam_{examId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                _logger.LogInformation("Successfully uploaded exam image: {FileName}", fileName);
                
                // Return relative path for database storage
                return $"/uploads/exams/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading exam image for exam ID: {ExamId}", examId);
                throw;
            }
        }

        public async Task<bool> DeleteExamImageAsync(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return true;

            try
            {
                var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", imagePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Successfully deleted exam image: {ImagePath}", imagePath);
                    return true;
                }

                _logger.LogWarning("Image file not found: {ImagePath}", imagePath);
                return true; // Consider it successful if file doesn't exist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam image: {ImagePath}", imagePath);
                return false;
            }
        }
    }
}
