using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MasterService.Application.Services;

namespace MasterService.API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IExamService _examService;
        private readonly ILogger<ExamImageController> _logger;

        public ExamImageController(IImageService imageService, IExamService examService, ILogger<ExamImageController> logger)
        {
            _imageService = imageService;
            _examService = examService;
            _logger = logger;
        }

        [HttpPost("{examId}/upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> UploadExamImage(int examId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var imagePath = await _imageService.UploadExamImageAsync(file, examId);
                
                if (string.IsNullOrEmpty(imagePath))
                    return BadRequest("Failed to upload image.");

                // Update the exam's ImageUrl in the database
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fullImageUrl = $"{baseUrl}{imagePath}";
                
                var updateSuccess = await _examService.UpdateExamImageUrlAsync(examId, fullImageUrl);
                if (!updateSuccess)
                {
                    _logger.LogWarning("Failed to update exam image URL in database for exam {ExamId}", examId);
                    // Still return success for upload, but log the issue
                }

                return Ok(new { 
                    message = "Image uploaded successfully",
                    imagePath = imagePath,
                    imageUrl = fullImageUrl,
                    examId = examId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for exam {ExamId}", examId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{examId}/delete-image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteExamImage(int examId, [FromBody] string imagePath)
        {
            try
            {
                var result = await _imageService.DeleteExamImageAsync(imagePath);
                
                if (!result)
                    return StatusCode(500, "Failed to delete image.");

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image for exam {ExamId}", examId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
