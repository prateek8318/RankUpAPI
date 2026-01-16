using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IConfiguration configuration, ILogger<ImageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> UploadProfilePhotoAsync(IFormFile? imageFile, int userId)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            try
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed.");
                }

                // Validate file size (max 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (imageFile.Length > maxFileSize)
                {
                    throw new InvalidOperationException("File size too large. Maximum allowed size is 5MB.");
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"user_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Return relative path
                var relativePath = Path.Combine("uploads", "profiles", fileName).Replace("\\", "/");
                _logger.LogInformation($"Profile photo uploaded for user {userId}: {relativePath}");
                
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading profile photo for user {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteProfilePhotoAsync(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return true;
            }

            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"Profile photo deleted: {imagePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting profile photo: {imagePath}");
                return false;
            }
        }
    }
}
