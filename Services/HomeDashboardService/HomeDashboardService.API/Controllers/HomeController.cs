using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Home Page Controller - Single endpoint for all home page data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IDashboardService dashboardService,
            ILogger<HomeController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete home page data
        /// </summary>
        /// <returns>Complete home page data including greeting, target exam, practice modes, continue practice, daily targets, rapid fire tests, free tests, and banners</returns>
        [HttpGet]
        public async Task<ActionResult<HomePageResponseDto>> GetHome()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetHomePageResponseAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving home page data");
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            // Try multiple claim names that JWT might use for NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                             ?? User.FindFirst("nameid") 
                             ?? User.FindFirst("sub") 
                             ?? User.FindFirst("UserId");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
