using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;
using Common.Language;
using ILanguageService = Common.Services.ILanguageService;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly ILanguageService _languageService;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, ILanguageService languageService)
        {
            _categoryService = categoryService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var categories = await _categoryService.GetCategoriesAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = categories,
                    language = currentLanguage,
                    message = "Categories fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new { success = false, message = "Error fetching categories" });
            }
        }

        [HttpGet("qualifications")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetQualifications([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var qualifications = await _categoryService.GetQualificationsAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = qualifications,
                    language = currentLanguage,
                    message = "Qualifications fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualifications");
                return StatusCode(500, new { success = false, message = "Error fetching qualifications" });
            }
        }

        [HttpGet("exam-categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetExamCategories([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var examCategories = await _categoryService.GetExamCategoriesAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = examCategories,
                    language = currentLanguage,
                    message = "Exam categories fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam categories");
                return StatusCode(500, new { success = false, message = "Error fetching exam categories" });
            }
        }

        [HttpGet("streams")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetStreams([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var streams = await _categoryService.GetStreamsAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = streams,
                    language = currentLanguage,
                    message = "Streams fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving streams");
                return StatusCode(500, new { success = false, message = "Error fetching streams" });
            }
        }

        [HttpGet("all-optimized")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAllCategoriesOptimized([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                
                var allData = await _categoryService.GetAllCategoriesOptimizedAsync(currentLanguage);

                return Ok(new
                {
                    success = true,
                    data = allData,
                    language = currentLanguage,
                    message = "All categories data fetched successfully (optimized)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories data (optimized)");
                return StatusCode(500, new { success = false, message = "Error fetching categories data" });
            }
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAllCategories([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                
                var categories = await _categoryService.GetCategoriesAsync(currentLanguage);
                var qualifications = await _categoryService.GetQualificationsAsync(currentLanguage);
                var examCategories = await _categoryService.GetExamCategoriesAsync(currentLanguage);
                var streams = await _categoryService.GetStreamsAsync(currentLanguage);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        categories = categories,
                        qualifications = qualifications,
                        examCategories = examCategories,
                        streams = streams
                    },
                    language = currentLanguage,
                    message = "All categories data fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories data");
                return StatusCode(500, new { success = false, message = "Error fetching categories data" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var category = await _categoryService.GetCategoryAsync(id, currentLanguage);
                if (category == null)
                    return NotFound();

                return Ok(new
                {
                    success = true,
                    data = category,
                    language = currentLanguage,
                    message = "Category fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category");
                return StatusCode(500, new { success = false, message = "Error fetching category" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _categoryService.CreateCategoryAsync(createDto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID in URL does not match the ID in the request body.");

                var result = await _categoryService.UpdateCategoryAsync(id, updateDto);
                if (result == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategoryStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _categoryService.ToggleCategoryStatusAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category status");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
