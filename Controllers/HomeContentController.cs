using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.DTOs;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Controllers
{
    /// <summary>
    /// User-facing home API – returns grouped home sections for the selected exam.
    /// </summary>
    [ApiController]
    [Route("api/home")]
    [AllowAnonymous]
    public class HomeContentController : ControllerBase
    {
        private readonly IHomeContentService _homeContentService;

        public HomeContentController(IHomeContentService homeContentService)
        {
            _homeContentService = homeContentService;
        }

        /// <summary>
        /// Get full home content for an exam.
        /// examId is optional; if not provided, common items (ExamId = null) will be returned.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<HomeContentResponseDto>> GetHome([FromQuery] int? examId, CancellationToken cancellationToken)
        {
            var response = await _homeContentService.GetHomeContentAsync(examId, cancellationToken);
            return Ok(response);
        }
    }
}

