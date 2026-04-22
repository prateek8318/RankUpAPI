using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SubscriptionService.Application.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folderName = "subscription-plans");
        bool DeleteImage(string imagePath);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageUploadService> _logger;
        private readonly string _uploadsFolder;
        private readonly string _baseUrl;

        public ImageUploadService(IConfiguration configuration, ILogger<ImageUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            _uploadsFolder = _configuration["ImageUpload:UploadsFolder"] ?? "wwwroot/uploads";
            _baseUrl = _configuration["ImageUpload:BaseUrl"] ?? "http://localhost:8000/uploads";
            
            // Ensure uploads folder exists
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folderName = "subscription-plans")
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    throw new ArgumentException("No file uploaded or file is empty");
                }

                // Validate file type
                if (!IsValidImageFile(imageFile))
                {
                    throw new ArgumentException("Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                }

                // Validate file size (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException("File size too large. Maximum allowed size is 5MB.");
                }

                // Create folder if it doesn't exist
                var targetFolder = Path.Combine(_uploadsFolder, folderName);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(targetFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Return relative URL
                var relativePath = Path.Combine(folderName, fileName).Replace("\\", "/");
                var fullUrl = $"{_baseUrl}/{relativePath}";

                _logger.LogInformation("Successfully uploaded image: {FileName}", fileName);
                return fullUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", imageFile?.FileName);
                throw;
            }
        }

        public bool DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return false;

                // Extract relative path from full URL
                var uri = new Uri(imagePath);
                var relativePath = uri.AbsolutePath.Trim('/');
                
                // Convert to local file path
                var fileName = Path.GetFileName(relativePath);
                var folderName = Path.GetDirectoryName(relativePath);
                
                if (!string.IsNullOrEmpty(folderName))
                {
                    var filePath = Path.Combine(_uploadsFolder, folderName, fileName);
                    
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogInformation("Successfully deleted image: {FileName}", fileName);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImagePath}", imagePath);
                return false;
            }
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
