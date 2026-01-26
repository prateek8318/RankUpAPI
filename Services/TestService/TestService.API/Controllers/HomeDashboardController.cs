using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestService.Application.DTOs;
using TestService.Application.Services;

namespace TestService.API.Controllers
{
    [Route("api/home/dashboard")]
    [ApiController]
    [Authorize]
    public class HomeDashboardController : ControllerBase
    {
        private readonly HomeDashboardService _service;

        public HomeDashboardController(HomeDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<HomeDashboardResponseDto>> GetDashboard()
        {
            // TODO: Get userId from claims
            var userId = 1; // Placeholder
            var dashboard = await _service.GetHomeDashboardAsync(userId);
            return Ok(dashboard);
        }

        [HttpGet("practice-modes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PracticeModeDto>>> GetPracticeModes()
        {
            var practiceModes = await _service.GetPracticeModesAsync();
            return Ok(practiceModes);
        }

        [HttpGet("exams")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams()
        {
            var exams = await _service.GetExamsAsync();
            return Ok(exams);
        }
    }
}
