using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Services;
using System.ComponentModel.DataAnnotations;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Admin Wallet Controller
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminWalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<AdminWalletController> _logger;

        public AdminWalletController(IWalletService walletService, ILogger<AdminWalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        /// <summary>
        /// Get overall wallet statistics
        /// </summary>
        /// <returns>Admin wallet statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<AdminWalletStatisticsDto>> GetStatistics()
        {
            try
            {
                var result = await _walletService.GetAdminWalletStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin wallet statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user wallet details by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User wallet information</returns>
        [HttpGet("users/{userId}")]
        public async Task<ActionResult<UserWalletDto>> GetUserWallet(int userId)
        {
            try
            {
                var result = await _walletService.GetUserWalletAsync(userId);
                if (result == null)
                    return NotFound($"Wallet for user {userId} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user wallet transactions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="transactionType">Filter by transaction type</param>
        /// <param name="status">Filter by status</param>
        /// <returns>Paginated wallet transactions</returns>
        [HttpGet("users/{userId}/transactions")]
        public async Task<ActionResult<WalletTransactionListResponseDto>> GetUserTransactions(
            int userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] WalletTransactionType? transactionType = null,
            [FromQuery] WalletTransactionStatus? status = null)
        {
            try
            {
                var request = new WalletTransactionListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TransactionType = transactionType,
                    Status = status
                };

                var result = await _walletService.GetWalletTransactionsAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet transactions for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Block user wallet
        /// </summary>
        /// <param name="request">Block wallet request</param>
        /// <returns>No content</returns>
        [HttpPost("block")]
        public async Task<ActionResult> BlockWallet([FromBody] BlockWalletRequestDto request)
        {
            try
            {
                var success = await _walletService.BlockWalletAsync(request.UserId, request.Reason);
                if (!success)
                    return NotFound($"Wallet for user {request.UserId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking wallet for user: {UserId}", request.UserId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Unblock user wallet
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>No content</returns>
        [HttpPost("users/{userId}/unblock")]
        public async Task<ActionResult> UnblockWallet(int userId)
        {
            try
            {
                var success = await _walletService.UnblockWalletAsync(userId);
                if (!success)
                    return NotFound($"Wallet for user {userId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking wallet for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get wallet transaction by ID
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("transactions/{transactionId}")]
        public async Task<ActionResult<WalletTransactionDto>> GetTransactionById(int transactionId)
        {
            try
            {
                var result = await _walletService.GetWalletTransactionByIdAsync(transactionId);
                if (result == null)
                    return NotFound($"Transaction with ID {transactionId} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet transaction: {TransactionId}", transactionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get wallet transaction by provider transaction ID
        /// </summary>
        /// <param name="providerTransactionId">Provider transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("transactions/by-provider/{providerTransactionId}")]
        public async Task<ActionResult<WalletTransactionDto>> GetTransactionByProviderId(string providerTransactionId)
        {
            try
            {
                var result = await _walletService.GetWalletTransactionByProviderIdAsync(providerTransactionId);
                if (result == null)
                    return NotFound($"Transaction with provider ID {providerTransactionId} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet transaction by provider ID: {ProviderTransactionId}", providerTransactionId);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class BlockWalletRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
