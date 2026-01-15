using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Video Manager Controller - FR-ADM-07
    /// Orchestrates calls to HomeDashboardService
    /// </summary>
    [Route("api/admin/videos")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminVideoController : ControllerBase
    {
        private readonly IHomeDashboardServiceClient _homeDashboardServiceClient;
        private readonly ILogger<AdminVideoController> _logger;

        public AdminVideoController(
            IHomeDashboardServiceClient homeDashboardServiceClient,
            ILogger<AdminVideoController> logger)
        {
            _homeDashboardServiceClient = homeDashboardServiceClient;
            _logger = logger;
        }

        [HttpGet("daily")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetDailyVideo()
        {
            try
            {
                var video = await _homeDashboardServiceClient.GetDailyVideoAsync();
                return Ok(new ApiResponseDto<object> { Success = true, Data = video });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily video");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> GetAllDailyVideos()
        {
            try
            {
                var videos = await _homeDashboardServiceClient.GetAllDailyVideosAsync();
                return Ok(new ApiResponseDto<object> { Success = true, Data = videos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all daily videos");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost("daily")]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateDailyVideo([FromBody] object createDto)
        {
            try
            {
                var video = await _homeDashboardServiceClient.CreateDailyVideoAsync(createDto);
                if (video == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create daily video" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = video });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily video");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("daily/{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateDailyVideo(int id, [FromBody] object updateDto)
        {
            try
            {
                var video = await _homeDashboardServiceClient.UpdateDailyVideoAsync(id, updateDto);
                if (video == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Daily video not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = video });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating daily video {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("daily/{id}")]
        public async Task<ActionResult> DeleteDailyVideo(int id)
        {
            try
            {
                var result = await _homeDashboardServiceClient.DeleteDailyVideoAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting daily video {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("banners")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetBanners()
        {
            try
            {
                var banners = await _homeDashboardServiceClient.GetBannersAsync();
                return Ok(new ApiResponseDto<object> { Success = true, Data = banners });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banners");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost("banners")]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateBanner([FromBody] object createDto)
        {
            try
            {
                var banner = await _homeDashboardServiceClient.CreateBannerAsync(createDto);
                if (banner == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create banner" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = banner });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating banner");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("banners/{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateBanner(int id, [FromBody] object updateDto)
        {
            try
            {
                var banner = await _homeDashboardServiceClient.UpdateBannerAsync(id, updateDto);
                if (banner == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Banner not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = banner });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating banner {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("banners/{id}")]
        public async Task<ActionResult> DeleteBanner(int id)
        {
            try
            {
                var result = await _homeDashboardServiceClient.DeleteBannerAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting banner {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
