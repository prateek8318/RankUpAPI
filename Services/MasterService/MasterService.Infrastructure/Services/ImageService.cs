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
                throw new InvalidOperationException("Please select an image file to upload.");

            try
            {
                // Validate file type - only JPG, JPEG, PNG, and WebP allowed
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Invalid file type: {FileExtension}", fileExtension);
                    throw new InvalidOperationException("Only JPG, JPEG, PNG, and WebP image files are allowed.");
                }

                // Validate file size (max 3MB for better performance)
                if (imageFile.Length > 3 * 1024 * 1024)
                {
                    _logger.LogWarning("File too large: {FileSize} bytes", imageFile.Length);
                    throw new InvalidOperationException("File size must be less than 3MB.");
                }

                // Validate MIME type
                var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
                if (!allowedMimeTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                {
                    _logger.LogWarning("Invalid MIME type: {MimeType}", imageFile.ContentType);
                    throw new InvalidOperationException("Invalid image file type.");
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "exams");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename with timestamp and random suffix
                var randomSuffix = new Random().Next(1000, 9999);
                var fileName = $"exam_{examId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{randomSuffix}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Verify file was saved and has content
                if (!new FileInfo(filePath).Exists || new FileInfo(filePath).Length == 0)
                {
                    throw new InvalidOperationException("Failed to save the image file.");
                }

                _logger.LogInformation("Successfully uploaded exam image: {FileName} ({FileSize} bytes)", fileName, imageFile.Length);
                
                // Return relative path for database storage
                return $"/uploads/exams/{fileName}";
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation exceptions
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading exam image for exam ID: {ExamId}", examId);
                throw new InvalidOperationException("Failed to upload image. Please try again.");
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

        public async Task<string?> UploadCountryImageAsync(IFormFile? imageFile, string iso2)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new InvalidOperationException("Please select an image file to upload.");

            try
            {
                // Validate file type - only JPG, JPEG, PNG, and WebP allowed
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Invalid file type: {FileExtension}", fileExtension);
                    throw new InvalidOperationException("Only JPG, JPEG, PNG, and WebP image files are allowed.");
                }

                // Validate file size (max 2MB for country flags)
                if (imageFile.Length > 2 * 1024 * 1024)
                {
                    _logger.LogWarning("File too large: {FileSize} bytes", imageFile.Length);
                    throw new InvalidOperationException("File size must be less than 2MB.");
                }

                // Validate MIME type
                var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
                if (!allowedMimeTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                {
                    _logger.LogWarning("Invalid MIME type: {MimeType}", imageFile.ContentType);
                    throw new InvalidOperationException("Invalid image file type.");
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "flags");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate filename with country code and extension
                var fileName = $"{iso2.ToLower()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Verify file was saved and has content
                if (!new FileInfo(filePath).Exists || new FileInfo(filePath).Length == 0)
                {
                    throw new InvalidOperationException("Failed to save the image file.");
                }

                _logger.LogInformation("Successfully uploaded country image: {FileName} ({FileSize} bytes)", fileName, imageFile.Length);
                
                // Return relative path for database storage
                return $"/uploads/flags/{fileName}";
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation exceptions
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading country image for country: {Iso2}", iso2);
                throw new InvalidOperationException("Failed to upload image. Please try again.");
            }
        }

        public async Task<bool> DeleteCountryImageAsync(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return true;

            try
            {
                var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", imagePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Successfully deleted country image: {ImagePath}", imagePath);
                    return true;
                }

                _logger.LogWarning("Image file not found: {ImagePath}", imagePath);
                return true; // Consider it successful if file doesn't exist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country image: {ImagePath}", imagePath);
                return false;
            }
        }
    }
}
