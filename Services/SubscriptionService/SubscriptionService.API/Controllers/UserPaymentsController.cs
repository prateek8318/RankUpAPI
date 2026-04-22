using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Payment Management Controller for Users
    /// </summary>
    [Route("api/user/payments")]
    [ApiController]
    [Authorize]
    public class UserPaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<UserPaymentsController> _logger;

        public UserPaymentsController(IPaymentService paymentService, ILogger<UserPaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Initiate payment for subscription
        /// </summary>
        /// <param name="paymentRequest">Payment initiation details</param>
        /// <returns>Payment initiation response</returns>
        [HttpPost("initiate")]
        public async Task<ActionResult<PaymentInitiationResponseDto>> InitiatePayment([FromBody] InitiatePaymentDto paymentRequest)
        {
            try
            {
                paymentRequest.UserId = GetUserIdFromToken();
                if (paymentRequest.UserId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                if (paymentRequest.PaymentProvider == default)
                    paymentRequest.PaymentProvider = PaymentProvider.Razorpay;

                var result = await _paymentService.InitiatePaymentAsync(paymentRequest);
                return Ok(new { success = true, data = result, message = "Payment initiated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order for plan: {PlanId}", paymentRequest.PlanId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Verify payment completion
        /// </summary>
        /// <param name="verificationRequest">Payment verification details</param>
        /// <returns>Payment verification result</returns>
        [HttpPost("verify")]
        public async Task<ActionResult<PaymentVerificationResultDto>> VerifyPayment([FromBody] VerifyPaymentDto verificationRequest)
        {
            try
            {
                verificationRequest.UserId = GetUserIdFromToken();
                if (verificationRequest.UserId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var result = await _paymentService.VerifyPaymentAsync(verificationRequest);
                return Ok(new { success = true, data = result, message = "Payment verified successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment: {TransactionId}", verificationRequest.TransactionId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Process refund for a payment
        /// </summary>
        /// <param name="refundRequest">Refund request details</param>
        /// <returns>Refund response</returns>
        [HttpPost("refund")]
        public async Task<ActionResult<RefundResponseDto>> ProcessRefund([FromBody] RefundRequestDto refundRequest)
        {
            try
            {
                var result = await _paymentService.ProcessRefundAsync(refundRequest);
                return Ok(new { success = true, data = result, message = "Refund processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", refundRequest.PaymentId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get payment history for the current user
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>User payment history</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Extract user ID from JWT token
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var result = await _paymentService.GetUserPaymentHistoryAsync(userId);
                return Ok(new { success = true, data = result, message = "Payment history retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
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
