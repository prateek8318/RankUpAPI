using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Wallet Controller
    /// </summary>
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        /// <summary>
        /// Get user wallet details
        /// </summary>
        /// <returns>User wallet information</returns>
        [HttpGet]
        public async Task<ActionResult<UserWalletDto>> GetWallet()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _walletService.GetUserWalletAsync(userId);
                if (result == null)
                {
                    // Create wallet if it doesn't exist
                    result = await _walletService.GetOrCreateUserWalletAsync(userId);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get wallet transaction history
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="transactionType">Filter by transaction type</param>
        /// <param name="status">Filter by status</param>
        /// <returns>Paginated wallet transactions</returns>
        [HttpGet("transactions")]
        public async Task<ActionResult<WalletTransactionListResponseDto>> GetTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] WalletTransactionType? transactionType = null,
            [FromQuery] WalletTransactionStatus? status = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

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
                _logger.LogError(ex, "Error retrieving wallet transactions for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get wallet statistics
        /// </summary>
        /// <returns>Wallet statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<WalletStatisticsDto>> GetWalletStatistics()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _walletService.GetWalletStatisticsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet statistics for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Initiate wallet recharge via Razorpay
        /// </summary>
        /// <param name="rechargeDto">Recharge details</param>
        /// <returns>Razorpay order details</returns>
        [HttpPost("recharge/initiate")]
        public async Task<ActionResult<RazorpayWalletRechargeResponseDto>> InitiateRecharge([FromBody] RechargeWalletRazorpayDto rechargeDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                rechargeDto.UserId = userId;
                var result = await _walletService.InitiateWalletRechargeAsync(rechargeDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating wallet recharge for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Verify wallet recharge payment
        /// </summary>
        /// <param name="verifyDto">Verification details</param>
        /// <returns>Verification result</returns>
        [HttpPost("recharge/verify")]
        public async Task<ActionResult<WalletRechargeVerificationResultDto>> VerifyRecharge([FromBody] VerifyWalletRechargeDto verifyDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                verifyDto.UserId = userId;
                var result = await _walletService.VerifyWalletRechargeAsync(verifyDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying wallet recharge for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Pay for subscription using wallet balance
        /// </summary>
        /// <param name="payDto">Payment details</param>
        /// <returns>Payment result</returns>
        [HttpPost("pay")]
        public async Task<ActionResult<WalletPaymentResultDto>> PayWithWallet([FromBody] PayWithWalletDto payDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                payDto.UserId = userId;
                var result = await _walletService.PayWithWalletAsync(payDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wallet payment for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if wallet has sufficient balance
        /// </summary>
        /// <param name="requiredAmount">Amount to check</param>
        /// <returns>Balance check result</returns>
        [HttpGet("check-balance")]
        public async Task<ActionResult<WalletBalanceCheckResultDto>> CheckBalance([FromQuery] decimal requiredAmount)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var checkDto = new CheckWalletBalanceDto
                {
                    UserId = userId,
                    RequiredAmount = requiredAmount
                };

                var result = await _walletService.CheckWalletBalanceAsync(checkDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking wallet balance for user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get specific wallet transaction by ID
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("transactions/{transactionId}")]
        public async Task<ActionResult<WalletTransactionDto>> GetTransactionById(int transactionId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _walletService.GetWalletTransactionByIdAsync(transactionId);
                if (result == null)
                    return NotFound($"Transaction with ID {transactionId} not found");

                // Verify the transaction belongs to the current user
                var wallet = await _walletService.GetUserWalletAsync(userId);
                if (wallet == null || wallet.Id != result.WalletId)
                    return Unauthorized("Access denied");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet transaction: {TransactionId}", transactionId);
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            // Extract user ID from JWT token claims
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
