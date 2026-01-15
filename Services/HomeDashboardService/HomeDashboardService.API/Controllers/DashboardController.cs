using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// User Dashboard Controller
    /// </summary>
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get user dashboard with all sections
        /// </summary>
        /// <returns>Complete dashboard data</returns>
        [HttpGet]
        public async Task<ActionResult<UserDashboardDto>> GetUserDashboard()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetUserDashboardAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get ongoing quizzes for the current user
        /// </summary>
        /// <returns>List of ongoing quizzes</returns>
        [HttpGet("ongoing-quizzes")]
        public async Task<ActionResult<List<OngoingQuizDto>>> GetOngoingQuizzes()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetOngoingQuizzesAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ongoing quizzes");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Resume an ongoing quiz
        /// </summary>
        /// <param name="quizAttemptId">Quiz attempt ID</param>
        /// <returns>Quiz attempt details</returns>
        [HttpGet("resume-quiz/{quizAttemptId}")]
        public async Task<ActionResult<QuizAttemptDto>> ResumeQuiz(int quizAttemptId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.ResumeQuizAsync(userId, quizAttemptId);
                if (result == null)
                    return NotFound("Quiz attempt not found or cannot be resumed");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming quiz: {QuizAttemptId}", quizAttemptId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get daily motivational video
        /// </summary>
        /// <returns>Daily video details</returns>
        [HttpGet("daily-video")]
        public async Task<ActionResult<DailyVideoDto>> GetDailyVideo()
        {
            try
            {
                var result = await _dashboardService.GetDailyVideoAsync();
                if (result == null)
                    return NotFound("No daily video available");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily video");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get recent quiz attempts
        /// </summary>
        /// <param name="limit">Number of recent attempts to retrieve (default: 10)</param>
        /// <returns>List of recent attempts</returns>
        [HttpGet("recent-attempts")]
        public async Task<ActionResult<List<RecentAttemptDto>>> GetRecentAttempts([FromQuery] int limit = 10)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetRecentAttemptsAsync(userId, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent attempts");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription banner information
        /// </summary>
        /// <returns>Subscription banner details</returns>
        [HttpGet("subscription-banner")]
        public async Task<ActionResult<SubscriptionBannerDto>> GetSubscriptionBanner()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetSubscriptionBannerAsync(userId);
                if (result == null)
                    return NotFound("No active subscription found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription banner");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get leaderboard for a quiz
        /// </summary>
        /// <param name="quizId">Quiz ID</param>
        /// <param name="limit">Number of entries to retrieve (default: 10)</param>
        /// <returns>Leaderboard entries</returns>
        [HttpGet("leaderboard/{quizId}")]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard(int quizId, [FromQuery] int limit = 10)
        {
            try
            {
                var result = await _dashboardService.GetLeaderboardAsync(quizId, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leaderboard for quiz: {QuizId}", quizId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get trending tests
        /// </summary>
        /// <param name="limit">Number of trending tests to retrieve (default: 10)</param>
        /// <returns>List of trending tests</returns>
        [HttpGet("trending-tests")]
        public async Task<ActionResult<List<TrendingTestDto>>> GetTrendingTests([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _dashboardService.GetTrendingTestsAsync(limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trending tests");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get notifications for the current user
        /// </summary>
        /// <param name="limit">Number of notifications to retrieve (default: 50)</param>
        /// <returns>List of notifications</returns>
        [HttpGet("notifications")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications([FromQuery] int limit = 50)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _dashboardService.GetNotificationsAsync(userId, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active dashboard banners
        /// </summary>
        /// <returns>List of active banners</returns>
        [HttpGet("banners")]
        public async Task<ActionResult<List<DashboardBannerDto>>> GetBanners()
        {
            try
            {
                var result = await _dashboardService.GetBannersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving banners");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active offer banners
        /// </summary>
        /// <returns>List of active offer banners</returns>
        [HttpGet("offer-banners")]
        public async Task<ActionResult<List<OfferBannerDto>>> GetOfferBanners()
        {
            try
            {
                var result = await _dashboardService.GetOfferBannersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving offer banners");
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
