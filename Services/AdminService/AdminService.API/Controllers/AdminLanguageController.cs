using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System.Text.Json;

namespace AdminService.API.Controllers
{
    [Route("api/admin/languages")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLanguageController : ControllerBase
    {
        private readonly IMasterServiceClient _masterServiceClient;
        private readonly ILogger<AdminLanguageController> _logger;

        public AdminLanguageController(
            IMasterServiceClient masterServiceClient,
            ILogger<AdminLanguageController> logger)
        {
            _masterServiceClient = masterServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateLanguage([FromBody] JsonElement createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Invalid request data" });

                var language = await _masterServiceClient.CreateLanguageAsync(createDto);
                if (language == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create language" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = language });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating language");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
