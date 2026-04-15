using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Razorpay Webhook Controller
    /// Handles payment notifications and wallet recharge confirmations
    /// </summary>
    [Route("api/webhooks")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IPaymentService paymentService,
            IWalletService walletService,
            IConfiguration configuration,
            ILogger<WebhookController> logger)
        {
            _paymentService = paymentService;
            _walletService = walletService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Razorpay payment webhook for subscription payments
        /// </summary>
        [HttpPost("razorpay")]
        public async Task<IActionResult> RazorpayWebhook()
        {
            try
            {
                _logger.LogInformation("Received Razorpay webhook");

                // Read the raw request body
                using var reader = new StreamReader(Request.Body);
                var payload = await reader.ReadToEndAsync();

                // Get Razorpay signature from header
                if (!Request.Headers.TryGetValue("X-Razorpay-Signature", out var signature))
                {
                    _logger.LogWarning("Missing Razorpay signature header");
                    return BadRequest("Missing signature");
                }

                // Verify webhook signature
                var secret = _configuration["Razorpay:WebhookSecret"];
                if (string.IsNullOrEmpty(secret))
                {
                    _logger.LogError("Razorpay webhook secret not configured");
                    return StatusCode(500, "Webhook not configured");
                }

                if (!VerifyRazorpaySignature(payload, signature, secret))
                {
                    _logger.LogWarning("Invalid Razorpay webhook signature");
                    return Unauthorized("Invalid signature");
                }

                // Parse webhook payload
                var webhookData = JsonSerializer.Deserialize<RazorpayWebhookPayload>(payload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (webhookData == null)
                {
                    _logger.LogWarning("Invalid webhook payload");
                    return BadRequest("Invalid payload");
                }

                // Process webhook based on event type
                await ProcessRazorpayWebhook(webhookData);

                _logger.LogInformation("Razorpay webhook processed successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Razorpay webhook");
                return StatusCode(500, "Webhook processing failed");
            }
        }

        /// <summary>
        /// Wallet recharge webhook
        /// </summary>
        [HttpPost("wallet")]
        public async Task<IActionResult> WalletWebhook()
        {
            try
            {
                _logger.LogInformation("Received wallet webhook");

                // Read the raw request body
                using var reader = new StreamReader(Request.Body);
                var payload = await reader.ReadToEndAsync();

                // Get signature from header
                if (!Request.Headers.TryGetValue("X-Signature", out var signature))
                {
                    _logger.LogWarning("Missing wallet webhook signature header");
                    return BadRequest("Missing signature");
                }

                // Verify webhook signature (similar to Razorpay verification)
                var secret = _configuration["Wallet:WebhookSecret"];
                if (string.IsNullOrEmpty(secret))
                {
                    _logger.LogError("Wallet webhook secret not configured");
                    return StatusCode(500, "Webhook not configured");
                }

                if (!VerifyWalletSignature(payload, signature, secret ?? string.Empty))
                {
                    _logger.LogWarning("Invalid wallet webhook signature");
                    return Unauthorized("Invalid signature");
                }

                // Parse and process wallet webhook
                var walletWebhookData = JsonSerializer.Deserialize<WalletWebhookPayload>(payload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (walletWebhookData == null)
                {
                    _logger.LogWarning("Invalid wallet webhook payload");
                    return BadRequest("Invalid payload");
                }

                await ProcessWalletWebhook(walletWebhookData);

                _logger.LogInformation("Wallet webhook processed successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wallet webhook");
                return StatusCode(500, "Webhook processing failed");
            }
        }

        private async Task ProcessRazorpayWebhook(RazorpayWebhookPayload webhookData)
        {
            switch (webhookData.Event?.ToLower())
            {
                case "payment.captured":
                    await HandlePaymentCaptured(webhookData);
                    break;

                case "payment.failed":
                    await HandlePaymentFailed(webhookData);
                    break;

                case "refund.processed":
                    await HandleRefundProcessed(webhookData);
                    break;

                default:
                    _logger.LogInformation("Unhandled Razorpay webhook event: {Event}", webhookData.Event);
                    break;
            }
        }

        private async Task HandlePaymentCaptured(RazorpayWebhookPayload webhookData)
        {
            var payment = webhookData.Payload?.Payment?.Entity;
            if (payment == null)
            {
                _logger.LogWarning("Invalid payment data in webhook");
                return;
            }

            _logger.LogInformation("Payment captured: {PaymentId} for order: {OrderId}", payment.Id, payment.OrderId);

            // Check if this is a wallet recharge payment
            if (payment.Notes?.ContainsKey("wallet_recharge") == true)
            {
                await HandleWalletRechargeSuccess(payment);
            }
            else
            {
                await HandleSubscriptionPaymentSuccess(payment);
            }
        }

        private async Task HandlePaymentFailed(RazorpayWebhookPayload webhookData)
        {
            var payment = webhookData.Payload?.Payment?.Entity;
            if (payment == null)
            {
                _logger.LogWarning("Invalid payment data in webhook");
                return;
            }

            _logger.LogInformation("Payment failed: {PaymentId} for order: {OrderId}", payment.Id, payment.OrderId);

            // Update payment status to failed
            // This would be handled by the payment service
        }

        private async Task HandleRefundProcessed(RazorpayWebhookPayload webhookData)
        {
            var refund = webhookData.Payload?.Refund?.Entity;
            if (refund == null)
            {
                _logger.LogWarning("Invalid refund data in webhook");
                return;
            }

            // Extract refund properties using JsonElement
            if (refund is JsonElement refundElement)
            {
                var refundId = refundElement.GetProperty("id").GetString();
                var paymentId = refundElement.GetProperty("payment_id").GetString();
                _logger.LogInformation("Refund processed: {RefundId} for payment: {PaymentId}", refundId, paymentId);
            }

            // Update refund status
            // This would be handled by the payment service
        }

        private async Task HandleWalletRechargeSuccess(RazorpayPaymentEntity payment)
        {
            try
            {
                // Extract user ID from payment notes
                if (payment.Notes?.TryGetValue("user_id", out var userIdValue) == true && 
                    int.TryParse(userIdValue.ToString(), out var userId))
                {
                    // Verify and process wallet recharge
                    var verifyDto = new VerifyWalletRechargeDto
                    {
                        UserId = userId,
                        RazorpayOrderId = payment.OrderId,
                        RazorpayPaymentId = payment.Id,
                        RazorpaySignature = "" // Not needed for webhook processing
                    };

                    // Process the recharge (this would be simplified for webhook)
                    _logger.LogInformation("Processing wallet recharge webhook for user: {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wallet recharge webhook");
            }
        }

        private async Task HandleSubscriptionPaymentSuccess(RazorpayPaymentEntity payment)
        {
            try
            {
                // Extract user ID and plan ID from payment notes
                if (payment.Notes?.TryGetValue("user_id", out var userIdValue) == true && 
                    int.TryParse(userIdValue.ToString(), out var userId) &&
                    payment.Notes?.TryGetValue("plan_id", out var planIdValue) == true && 
                    int.TryParse(planIdValue.ToString(), out var planId))
                {
                    _logger.LogInformation("Processing subscription payment webhook for user: {UserId}, plan: {PlanId}", userId, planId);
                    
                    // Process subscription payment
                    // This would be handled by the payment service
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing subscription payment webhook");
            }
        }

        private async Task ProcessWalletWebhook(WalletWebhookPayload webhookData)
        {
            switch (webhookData.Event?.ToLower())
            {
                case "wallet.recharge.success":
                    await HandleWalletRechargeWebhook(webhookData);
                    break;

                case "wallet.payment.success":
                    await HandleWalletPaymentWebhook(webhookData);
                    break;

                default:
                    _logger.LogInformation("Unhandled wallet webhook event: {Event}", webhookData.Event);
                    break;
            }
        }

        private async Task HandleWalletRechargeWebhook(WalletWebhookPayload webhookData)
        {
            // Process wallet recharge webhook
            _logger.LogInformation("Processing wallet recharge webhook: {TransactionId}", webhookData.TransactionId);
        }

        private async Task HandleWalletPaymentWebhook(WalletWebhookPayload webhookData)
        {
            // Process wallet payment webhook
            _logger.LogInformation("Processing wallet payment webhook: {TransactionId}", webhookData.TransactionId);
        }

        private bool VerifyRazorpaySignature(string payload, string signature, string secret)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(secret);
                var payloadBytes = Encoding.UTF8.GetBytes(payload);

                using var hmac = new HMACSHA256(keyBytes);
                var computedHash = hmac.ComputeHash(payloadBytes);
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

                return computedSignature == signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Razorpay signature");
                return false;
            }
        }

        private bool VerifyWalletSignature(string payload, string signature, string secret)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(secret);
                var payloadBytes = Encoding.UTF8.GetBytes(payload);

                using var hmac = new HMACSHA256(keyBytes);
                var computedHash = hmac.ComputeHash(payloadBytes);
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

                return computedSignature == signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying wallet signature");
                return false;
            }
        }
    }

    // Razorpay Webhook DTOs
    public class RazorpayWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public int CreatedAt { get; set; }
        public RazorpayWebhookData Payload { get; set; } = new();
    }

    public class RazorpayWebhookData
    {
        public RazorpayWebhookPayment Payment { get; set; } = new();
        public RazorpayWebhookRefund Refund { get; set; } = new();
    }

    public class RazorpayWebhookPayment
    {
        public RazorpayPaymentEntity Entity { get; set; } = new();
    }

    public class RazorpayWebhookRefund
    {
        public object Entity { get; set; } = new();
    }

    public class RazorpayPaymentEntity
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, object>? Notes { get; set; }
    }

    // Wallet Webhook DTOs
    public class WalletWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, object>? Data { get; set; }
    }
}
