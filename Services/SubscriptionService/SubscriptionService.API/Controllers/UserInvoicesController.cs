using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Invoice Controller for Users
    /// </summary>
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Download invoice for a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns>Invoice PDF file</returns>
        [HttpGet("download/{subscriptionId}")]
        public async Task<ActionResult> DownloadInvoice(int subscriptionId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var downloadDto = new DownloadInvoiceDto
                {
                    SubscriptionId = subscriptionId,
                    UserId = userId
                };

                var result = await _invoiceService.DownloadInvoiceAsync(downloadDto);
                
                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                if (result.PdfData == null)
                    return NotFound("Invoice not found");

                return File(result.PdfData, result.ContentType!, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading invoice for subscription: {SubscriptionId}", subscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get invoice history for the current user
        /// </summary>
        /// <returns>User invoice history</returns>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoiceHistory()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _invoiceService.GetUserInvoicesAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice history for user");
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
