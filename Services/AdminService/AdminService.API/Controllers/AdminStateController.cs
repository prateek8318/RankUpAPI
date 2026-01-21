using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System.Text.Json;

namespace AdminService.API.Controllers
{
    [Route("api/admin/states")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminStateController : ControllerBase
    {
        private readonly IMasterServiceClient _masterServiceClient;
        private readonly ILogger<AdminStateController> _logger;

        public AdminStateController(
            IMasterServiceClient masterServiceClient,
            ILogger<AdminStateController> logger)
        {
            _masterServiceClient = masterServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateState([FromBody] JsonElement createDto, [FromQuery] int? languageId = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Invalid request data" });

                var state = await _masterServiceClient.CreateStateAsync(createDto);
                if (state == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create state" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = state });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating state");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
