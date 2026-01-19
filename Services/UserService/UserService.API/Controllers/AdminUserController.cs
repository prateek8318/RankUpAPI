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
                return Ok(ApiResponse.CreateSuccess(
                    $"Retrieved {users.Count()} users successfully",
                    users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to retrieve users list. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse.CreateNotFound(
                        $"User with ID {id} was not found",
                        ErrorCodes.USER_NOT_FOUND));
                }

                return Ok(ApiResponse.CreateSuccess(
                    $"User with ID {id} retrieved successfully",
                    user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    $"Unable to retrieve user with ID {id}. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetTotalUsersCount()
        {
            try
            {
                var totalUsers = await _userService.GetTotalUsersCountAsync();
                return Ok(ApiResponse.CreateSuccess(
                    "Total users count retrieved successfully",
                    new { totalUsers }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total users count");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to retrieve total users count. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpGet("daily-active-count")]
        public async Task<ActionResult> GetDailyActiveUsersCount()
        {
            try
            {
                var dailyActiveUsers = await _userService.GetDailyActiveUsersCountAsync();
                return Ok(ApiResponse.CreateSuccess(
                    "Daily active users count retrieved successfully",
                    new { dailyActiveUsers }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily active users count");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to retrieve daily active users count. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }
    }
}
