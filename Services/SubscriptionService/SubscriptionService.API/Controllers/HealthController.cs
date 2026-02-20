using Microsoft.AspNetCore.Mvc;

namespace SubscriptionService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", service = "SubscriptionService", timestamp = DateTime.UtcNow });
        }
    }
}
