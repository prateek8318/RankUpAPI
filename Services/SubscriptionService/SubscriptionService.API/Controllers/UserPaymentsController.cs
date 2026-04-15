using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Payment Controller for Users
    /// </summary>
    [Route("api/user/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a Razorpay order for subscription payment
        /// </summary>
        /// <param name="createOrderDto">Order creation details</param>
        /// <returns>Razorpay order details</returns>
        [HttpPost("create-order")]
        [HttpPost("initiate")]
        public async Task<ActionResult<PaymentInitiationResponseDto>> CreateRazorpayOrder([FromBody] InitiatePaymentDto createOrderDto)
        {
            try
            {
                createOrderDto.UserId = GetUserIdFromToken();
                if (createOrderDto.UserId == 0)
                    return Unauthorized("Invalid user token");

                if (createOrderDto.PaymentProvider == 0)
                    createOrderDto.PaymentProvider = PaymentProvider.Razorpay;

                var result = await _paymentService.InitiatePaymentAsync(createOrderDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order for plan: {PlanId}", createOrderDto.PlanId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Verify Razorpay payment after completion
        /// </summary>
        /// <param name="verifyPaymentDto">Payment verification details</param>
        /// <returns>Payment verification result</returns>
        [HttpPost("verify")]
        public async Task<ActionResult<PaymentVerificationResultDto>> VerifyPayment([FromBody] VerifyPaymentDto verifyPaymentDto)
        {
            try
            {
                verifyPaymentDto.UserId = GetUserIdFromToken();
                if (verifyPaymentDto.UserId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _paymentService.VerifyPaymentAsync(verifyPaymentDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment: {TransactionId}", verifyPaymentDto.TransactionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Process refund for a payment
        /// </summary>
        /// <param name="refundRequestDto">Refund request details</param>
        /// <returns>Refund response</returns>
        [HttpPost("refund")]
        public async Task<ActionResult<RefundResponseDto>> ProcessRefund([FromBody] RefundRequestDto refundRequestDto)
        {
            try
            {
                var result = await _paymentService.ProcessRefundAsync(refundRequestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment: {PaymentId}", refundRequestDto.PaymentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get payment history for the current user
        /// </summary>
        /// <returns>User payment history</returns>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory()
        {
            try
            {
                // Extract user ID from JWT token
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _paymentService.GetUserPaymentHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user");
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            // This is a simplified version. In production, you should properly validate the JWT token
            // and extract the user ID from claims
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
