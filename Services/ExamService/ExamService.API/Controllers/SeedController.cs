using ExamService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.API.Controllers
{
    [Route("api/seed")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ExamDataSeedService _seedService;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ExamDataSeedService seedService, ILogger<SeedController> logger)
        {
            _seedService = seedService;
            _logger = logger;
        }

        /// <summary>
        /// Seed exams data (National and International) with qualifications and streams
        /// </summary>
        /// <param name="clearExisting">If true, clears existing data before seeding</param>
        [HttpPost("exams")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedExams([FromQuery] bool clearExisting = false)
        {
            try
            {
                await _seedService.SeedExamsAsync(clearExisting);
                return Ok(new { message = "Exams seeded successfully", clearExisting });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding exams");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
