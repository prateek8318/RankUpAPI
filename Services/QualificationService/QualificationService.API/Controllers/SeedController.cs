using QualificationService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QualificationService.API.Controllers
{
    [Route("api/seed")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly DataSeedService _seedService;
        private readonly ILogger<SeedController> _logger;

        public SeedController(DataSeedService seedService, ILogger<SeedController> logger)
        {
            _seedService = seedService;
            _logger = logger;
        }

        /// <summary>
        /// Seed streams and qualifications data
        /// </summary>
        /// <param name="clearExisting">If true, clears existing data before seeding</param>
        [HttpPost("qualifications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedQualifications([FromQuery] bool clearExisting = false)
        {
            try
            {
                await _seedService.SeedDataAsync(clearExisting);
                return Ok(new { message = "Data seeded successfully", clearExisting });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding data");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
