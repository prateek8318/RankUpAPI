using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System.Text.Json;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin User Management Controller - FR-ADM-08
    /// Orchestrates calls to UserService
    /// </summary>
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(
            IUserServiceClient userServiceClient,
            ISubscriptionServiceClient subscriptionServiceClient,
            ILogger<AdminUserController> logger)
        {
            _userServiceClient = userServiceClient;
            _subscriptionServiceClient = subscriptionServiceClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<object>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var users = await _userServiceClient.GetAllUsersAsync(page, pageSize);
                var totalCount = await _userServiceClient.GetTotalUsersCountAsync();
                
                var enhancedUsers = new List<object>();
                
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        // Get subscription information for each user
                        var userId = GetUserIdFromUser(user);
                        if (userId.HasValue)
                        {
                            var subscriptionData = await _subscriptionServiceClient.GetUserSubscriptionDetailsAsync(userId.Value);
                            
                            var enhancedUser = new
                            {
                                // User basic info
                                User = user,
                                
                                // Subscription information
                                Subscription = subscriptionData,
                                
                                // Extract key subscription details for easier access
                                HasActiveSubscription = subscriptionData != null,
                                PlanName = GetPlanName(subscriptionData),
                                PlanPrice = GetPlanPrice(subscriptionData),
                                SubscriptionStatus = GetSubscriptionStatus(subscriptionData),
                                ValidTill = GetValidTill(subscriptionData),
                                DaysLeft = GetDaysLeft(subscriptionData)
                            };
                            
                            enhancedUsers.Add(enhancedUser);
                        }
                        else
                        {
                            enhancedUsers.Add(user);
                        }
                    }
                }
                
                return Ok(new PaginatedResponseDto<object>
                {
                    Items = enhancedUsers,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetUserById(int id)
        {
            try
            {
                var user = await _userServiceClient.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "User not found" });

                // Get subscription information for the user
                var subscriptionData = await _subscriptionServiceClient.GetUserSubscriptionDetailsAsync(id);
                
                // Create enhanced user object with subscription info
                var enhancedUser = new
                {
                    // User basic info
                    User = user,
                    
                    // Subscription information
                    Subscription = subscriptionData,
                    
                    // Extract key subscription details for easier access
                    HasActiveSubscription = subscriptionData != null,
                    PlanName = GetPlanName(subscriptionData),
                    PlanPrice = GetPlanPrice(subscriptionData),
                    SubscriptionStatus = GetSubscriptionStatus(subscriptionData),
                    ValidTill = GetValidTill(subscriptionData),
                    DaysLeft = GetDaysLeft(subscriptionData)
                };

                return Ok(new ApiResponseDto<object> { Success = true, Data = enhancedUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateUser(int id, [FromBody] object updateDto)
        {
            try
            {
                var user = await _userServiceClient.UpdateUserAsync(id, updateDto);
                if (user == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "User not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userServiceClient.DeleteUserAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableUser(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _userServiceClient.EnableDisableUserAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enabling/disabling user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private string? GetPlanName(object? subscriptionData)
        {
            if (subscriptionData == null) return null;
            
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(subscriptionData);
                var subscription = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (subscription.TryGetProperty("subscriptionPlan", out var planProp) && planProp.ValueKind == JsonValueKind.Object)
                {
                    if (planProp.TryGetProperty("nameEn", out var nameProp) || 
                        planProp.TryGetProperty("name", out nameProp))
                    {
                        return nameProp.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting plan name from subscription data");
            }
            
            return null;
        }

        private decimal? GetPlanPrice(object? subscriptionData)
        {
            if (subscriptionData == null) return null;
            
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(subscriptionData);
                var subscription = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (subscription.TryGetProperty("subscriptionPlan", out var planProp) && planProp.ValueKind == JsonValueKind.Object)
                {
                    if (planProp.TryGetProperty("price", out var priceProp))
                    {
                        return priceProp.GetDecimal();
                    }
                }
                
                // Fallback to amountPaid if plan price is not available
                if (subscription.TryGetProperty("amountPaid", out var amountProp))
                {
                    return amountProp.GetDecimal();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting plan price from subscription data");
            }
            
            return null;
        }

        private string? GetSubscriptionStatus(object? subscriptionData)
        {
            if (subscriptionData == null) return null;
            
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(subscriptionData);
                var subscription = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (subscription.TryGetProperty("status", out var statusProp))
                {
                    return statusProp.GetString();
                }
                
                if (subscription.TryGetProperty("currentStatus", out var currentStatusProp))
                {
                    return currentStatusProp.GetString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting subscription status from subscription data");
            }
            
            return null;
        }

        private DateTime? GetValidTill(object? subscriptionData)
        {
            if (subscriptionData == null) return null;
            
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(subscriptionData);
                var subscription = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (subscription.TryGetProperty("validTill", out var validTillProp))
                {
                    return validTillProp.GetDateTime();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting valid till date from subscription data");
            }
            
            return null;
        }

        private int? GetDaysLeft(object? subscriptionData)
        {
            if (subscriptionData == null) return null;
            
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(subscriptionData);
                var subscription = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (subscription.TryGetProperty("daysLeft", out var daysLeftProp))
                {
                    return daysLeftProp.GetInt32();
                }
                
                if (subscription.TryGetProperty("daysUntilExpiry", out var daysUntilExpiryProp))
                {
                    return daysUntilExpiryProp.GetInt32();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting days left from subscription data");
            }
            
            return null;
        }

        private int? GetUserIdFromUser(object user)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(user);
                var userElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
                
                if (userElement.TryGetProperty("id", out var idProp))
                {
                    return idProp.GetInt32();
                }
                
                if (userElement.TryGetProperty("userId", out var userIdProp))
                {
                    return userIdProp.GetInt32();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting user ID from user data");
            }
            
            return null;
        }
    }
}
