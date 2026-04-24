using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminUserController(
            IUserService userService,
            ILogger<AdminUserController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var users = (await _userService.GetAllUsersAsync(page, pageSize)).ToList();
                var subscriptionMap = await GetSubscriptionMapAsync();

                foreach (var user in users)
                {
                    if (subscriptionMap.TryGetValue(user.Id, out var subscription))
                    {
                        ApplySubscriptionData(user, subscription);
                    }
                }

                return Ok(ApiResponse.CreateSuccess(
                    $"Retrieved {users.Count} users successfully",
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

                var subscription = await GetSubscriptionByUserIdAsync(id);
                if (subscription != null)
                {
                    ApplySubscriptionData(user, subscription);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] AdminUserUpdateRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserByAdminAsync(id, request);
                var subscription = await GetSubscriptionByUserIdAsync(id);
                if (subscription != null)
                {
                    ApplySubscriptionData(user, subscription);
                }

                return Ok(ApiResponse.CreateSuccess($"User with ID {id} updated successfully", user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    $"User with ID {id} was not found",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    $"Unable to update user with ID {id}. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.SoftDeleteUserByAdminAsync(id);
                if (!deleted)
                {
                    return NotFound(ApiResponse.CreateNotFound(
                        $"User with ID {id} was not found",
                        ErrorCodes.USER_NOT_FOUND));
                }

                return Ok(ApiResponse.CreateSuccess($"User with ID {id} deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    $"Unable to delete user with ID {id}. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult<UserDto>> EnableDisableUser(int id, [FromBody] EnableDisableUserRequest request)
        {
            try
            {
                var user = await _userService.SetUserActiveStatusAsync(id, request.IsActive);

                // Keep subscription status consistent with user status.
                if (!request.IsActive)
                {
                    await BlockUserInSubscriptionServiceAsync(id, request.Reason ?? "Blocked by admin from user management");
                }

                var subscription = await GetSubscriptionByUserIdAsync(id);
                if (subscription != null)
                {
                    ApplySubscriptionData(user, subscription);
                }

                return Ok(ApiResponse.CreateSuccess($"User status updated successfully", user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    $"User with ID {id} was not found",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating active status for user {UserId}", id);
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to update user status. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpPost("{id}/subscription/cancel")]
        public async Task<ActionResult> CancelUserSubscription(int id, [FromBody] CancelSubscriptionRequest? request)
        {
            try
            {
                request ??= new CancelSubscriptionRequest();
                var subscriptionId = request.SubscriptionId;
                if (!subscriptionId.HasValue)
                {
                    var subscription = await GetSubscriptionByUserIdAsync(id);
                    subscriptionId = subscription?.SubscriptionId;
                }

                if (!subscriptionId.HasValue || subscriptionId.Value <= 0)
                {
                    return NotFound(ApiResponse.CreateNotFound("Active subscription not found for this user", ErrorCodes.RESOURCE_NOT_FOUND));
                }

                var payload = new { subscriptionId = subscriptionId.Value, reason = request.Reason };
                var result = await PostJsonToSubscriptionServiceAsync("/api/admin/user-subscriptions/cancel", payload);
                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, ApiResponse.CreateInternalServerError(
                        "Unable to cancel subscription at this time.",
                        ErrorCodes.EXTERNAL_SERVICE_ERROR,
                        result.RawBody));
                }

                return Ok(ApiResponse.CreateSuccess("Subscription cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription for user {UserId}", id);
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to cancel subscription at this time.",
                    ErrorCodes.EXTERNAL_SERVICE_ERROR));
            }
        }

        [HttpPost("{id}/subscription/refund")]
        public async Task<ActionResult> RefundUserSubscription(int id, [FromBody] RefundUserRequest request)
        {
            if (request.PaymentId <= 0)
            {
                return BadRequest(ApiResponse.CreateBadRequest("Valid paymentId is required", ErrorCodes.MISSING_REQUIRED_FIELDS));
            }

            var payload = new
            {
                paymentId = request.PaymentId,
                amount = request.Amount,
                reason = string.IsNullOrWhiteSpace(request.Reason) ? $"Refund initiated by admin for user {id}" : request.Reason
            };

            var result = await PostJsonToSubscriptionServiceAsync("/api/admin/adminpayments/refund", payload);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, ApiResponse.CreateInternalServerError(
                    "Unable to process refund at this time.",
                    ErrorCodes.EXTERNAL_SERVICE_ERROR,
                    result.RawBody));
            }

            return Ok(ApiResponse.CreateSuccess("Refund processed successfully"));
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

        private async Task<Dictionary<int, SubscriptionUserManagementDto>> GetSubscriptionMapAsync()
        {
            try
            {
                var client = CreateSubscriptionServiceClient();
                var response = await client.GetAsync("/api/admin/user-subscriptions/user-management/users");
                if (!response.IsSuccessStatusCode)
                {
                    return new Dictionary<int, SubscriptionUserManagementDto>();
                }

                var raw = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<SubscriptionListResponse>(raw, JsonSerializerOptions.Web);
                var items = wrapper?.Data ?? new List<SubscriptionUserManagementDto>();
                return items
                    .Where(x => x.Id > 0)
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.First());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fetch subscription map for admin users list");
                return new Dictionary<int, SubscriptionUserManagementDto>();
            }
        }

        private async Task<SubscriptionUserManagementDto?> GetSubscriptionByUserIdAsync(int userId)
        {
            var map = await GetSubscriptionMapAsync();
            return map.TryGetValue(userId, out var item) ? item : null;
        }

        private void ApplySubscriptionData(UserDto user, SubscriptionUserManagementDto subscription)
        {
            user.HasActiveSubscription = subscription.HasActiveSubscription;
            user.SubscriptionId = subscription.SubscriptionId;
            user.SubscriptionPlanName = subscription.PlanName;
            user.SubscriptionExpiryDate = subscription.SubscriptionExpiryDate;
            user.DaysRemaining = subscription.DaysRemaining;
            user.SubscriptionAmount = subscription.SubscriptionAmount;
            user.SubscriptionStatus = subscription.Status;
        }

        private HttpClient CreateSubscriptionServiceClient()
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ServiceUrls:SubscriptionService"] ?? "http://localhost:56925";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
            var token = GetBearerTokenFromRequest();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private string? GetBearerTokenFromRequest()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                return null;

            var headerValue = authHeader.ToString();
            if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return headerValue["Bearer ".Length..].Trim();
            }

            return null;
        }

        private async Task BlockUserInSubscriptionServiceAsync(int userId, string reason)
        {
            var payload = new
            {
                userId,
                reason,
                block = true
            };

            await PostJsonToSubscriptionServiceAsync("/api/admin/user-subscriptions/user-management/block", payload);
        }

        private async Task<ProxyOperationResult> PostJsonToSubscriptionServiceAsync(string route, object payload)
        {
            try
            {
                var client = CreateSubscriptionServiceClient();
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(route, content);
                var raw = await response.Content.ReadAsStringAsync();
                return new ProxyOperationResult(response.IsSuccessStatusCode, (int)response.StatusCode, raw);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed calling SubscriptionService route {Route}", route);
                return new ProxyOperationResult(false, StatusCodes.Status502BadGateway, null);
            }
        }

        private sealed record ProxyOperationResult(bool Success, int StatusCode, string? RawBody);
        private sealed class SubscriptionListResponse
        {
            public bool Success { get; set; }
            public List<SubscriptionUserManagementDto>? Data { get; set; }
            public string? Message { get; set; }
        }

        private sealed class SubscriptionUserManagementDto
        {
            public int Id { get; set; }
            public string? Status { get; set; }
            public bool HasActiveSubscription { get; set; }
            public string? PlanName { get; set; }
            public DateTime? SubscriptionExpiryDate { get; set; }
            public int? SubscriptionId { get; set; }
            public decimal? SubscriptionAmount { get; set; }
            public int DaysRemaining { get; set; }
        }
    }

    public class EnableDisableUserRequest
    {
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }

    public class CancelSubscriptionRequest
    {
        public int? SubscriptionId { get; set; }
        public string? Reason { get; set; }
    }

    public class RefundUserRequest
    {
        public int PaymentId { get; set; }
        public decimal? Amount { get; set; }
        public string? Reason { get; set; }
    }
}
