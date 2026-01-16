using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System.Text.Json;

namespace AdminService.API.Controllers
{
    [Route("api/admin/qualifications")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminQualificationController : ControllerBase
    {
        private readonly IQualificationServiceClient _qualificationServiceClient;
        private readonly ILogger<AdminQualificationController> _logger;

        public AdminQualificationController(
            IQualificationServiceClient qualificationServiceClient,
            ILogger<AdminQualificationController> logger)
        {
            _qualificationServiceClient = qualificationServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateQualification([FromBody] JsonElement createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Invalid request data" });

                var qualification = await _qualificationServiceClient.CreateQualificationAsync(createDto);
                if (qualification == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create qualification" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = qualification });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating qualification");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
