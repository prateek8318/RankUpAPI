using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin,Service")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(IUserService userService, ILogger<AdminUserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(page, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetTotalUsersCount()
        {
            try
            {
                var totalUsers = await _userService.GetTotalUsersCountAsync();
                return Ok(new { totalUsers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total users count");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("daily-active-count")]
        public async Task<ActionResult> GetDailyActiveUsersCount()
        {
            try
            {
                var dailyActiveUsers = await _userService.GetDailyActiveUsersCountAsync();
                return Ok(new { dailyActiveUsers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily active users count");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
