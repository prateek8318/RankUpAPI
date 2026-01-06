using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RankUpAPI.Areas.Admin.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = "Admin")] // Only admins can access these endpoints
    public class AdminController : ControllerBase
    {
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            // In a real application, fetch all users from database
            var users = new[] 
            {
                new { Id = 1, MobileNumber = "1234567890", Role = "User" },
                new { Id = 2, MobileNumber = "9876543210", Role = "Admin" }
            };
            
            return Ok(users);
        }

        [HttpPost("users/{userId}/promote")]
        public IActionResult PromoteUser(int userId)
        {
            // In a real application, update user role to admin in database
            return Ok(new { Success = true, Message = $"User {userId} has been promoted to admin" });
        }
    }
}
