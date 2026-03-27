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
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var categories = await _categoryService.GetCategoriesAsync(currentLanguage);
                
                return Ok(new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Success = true,
                    Data = categories,
                    Language = currentLanguage,
                    Message = "Categories fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Success = false,
                    Message = "Error fetching categories",
                    Error = ex.Message
                });
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
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var category = await _categoryService.GetCategoryAsync(id, currentLanguage);
                if (category == null)
                {
                    return NotFound(new ApiResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = $"Category with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<CategoryDto>
                {
                    Success = true,
                    Data = category,
                    Language = currentLanguage,
                    Message = "Category fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category");
                return StatusCode(500, new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Error fetching category",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(CreateCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Invalid model data",
                        Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }

                var category = await _categoryService.CreateCategoryAsync(createDto);
                
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new ApiResponse<CategoryDto>
                {
                    Success = true,
                    Data = category,
                    Language = _languageService.GetCurrentLanguage(),
                    Message = "Category created successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Validation failed",
                    Error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Validation failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Error creating category",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(new ApiResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "ID mismatch",
                        Error = "ID in URL does not match the ID in the request body"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Invalid model data",
                        Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }

                var result = await _categoryService.UpdateCategoryAsync(id, updateDto);
                if (result == null)
                {
                    return NotFound(new ApiResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = $"Category with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<CategoryDto>
                {
                    Success = true,
                    Data = result,
                    Language = _languageService.GetCurrentLanguage(),
                    Message = "Category updated successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Validation failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Error updating category",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = $"Category with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Language = _languageService.GetCurrentLanguage(),
                    Message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting category",
                    Error = ex.Message
                });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategoryStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _categoryService.ToggleCategoryStatusAsync(id, isActive);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = $"Category with ID {id} not found"
                    });
                }

                var message = isActive ? "Category activated successfully" : "Category deactivated successfully";
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Language = _languageService.GetCurrentLanguage(),
                    Message = message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category status");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error updating category status",
                    Error = ex.Message
                });
            }
        }
    }
}
