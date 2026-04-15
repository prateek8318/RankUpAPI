using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Admin Payment Management Controller
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminPaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<AdminPaymentsController> _logger;

        public AdminPaymentsController(
            IPaymentService paymentService,
            ILogger<AdminPaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get payment statistics dashboard
        /// </summary>
        /// <returns>Payment statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<SubscriptionStatisticsDto>> GetStatistics()
        {
            try
            {
                var result = await _paymentService.GetStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get paginated list of payments with filters
        /// </summary>
        /// <param name="request">Pagination and filter parameters</param>
        /// <returns>Paginated payments list</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetPagedPayments([FromQuery] AdminPaymentListRequestDto request)
        {
            try
            {
                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentTransactionDto>> GetPaymentById(int id)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Lookup by payment ID is not implemented in the current payment service.");
        }

        /// <summary>
        /// Get payments by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>User payments list</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetPaymentsByUserId(
            int userId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    UserId = userId
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments for user: {UserId}", userId);
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
        /// Update payment status manually
        /// </summary>
        /// <param name="request">Update payment status request</param>
        /// <returns>Updated payment details</returns>
        [HttpPut("update-status")]
        public async Task<ActionResult<PaymentTransactionDto>> UpdatePaymentStatus([FromBody] UpdatePaymentStatusDto request)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Manual payment status updates are not implemented in the current payment service.");
        }

        /// <summary>
        /// Get failed payments
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Failed payments list</returns>
        [HttpGet("failed")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetFailedPayments(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = PaymentStatus.Failed
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get pending payments
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Pending payments list</returns>
        [HttpGet("pending")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetPendingPayments(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = PaymentStatus.Pending
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get successful payments
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Successful payments list</returns>
        [HttpGet("successful")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetSuccessfulPayments(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = PaymentStatus.Success
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving successful payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get refunded payments
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Refunded payments list</returns>
        [HttpGet("refunded")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetRefundedPayments(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = PaymentStatus.Refunded
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refunded payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get payments by date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Payments in date range</returns>
        [HttpGet("date-range")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetPaymentsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var request = new AdminPaymentListRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CreatedDateFrom = startDate,
                    CreatedDateTo = endDate
                };

                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments by date range: {StartDate} - {EndDate}", startDate, endDate);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Export payments to CSV
        /// </summary>
        /// <param name="request">Export filter parameters</param>
        /// <returns>CSV file</returns>
        [HttpPost("export")]
        public async Task<ActionResult> ExportPaymentsToCsv([FromBody] AdminPaymentListRequestDto request)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "CSV export is not implemented in the current payment service.");
        }

        /// <summary>
        /// Get payment analytics
        /// </summary>
        /// <param name="days">Number of days to analyze (default: 30)</param>
        /// <returns>Payment analytics data</returns>
        [HttpGet("analytics")]
        public async Task<ActionResult<PaymentAnalyticsDto>> GetPaymentAnalytics([FromQuery] int days = 30)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Payment analytics are not implemented in the current payment service.");
        }
    }

    // Additional DTOs for admin payment operations
    public class PagedResponseDto<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }

    public class PaymentAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();
        public List<PaymentMethodStatisticsDto> PaymentMethods { get; set; } = new();
        public List<PaymentStatusStatisticsDto> PaymentStatuses { get; set; } = new();
    }

    public class PaymentStatusStatisticsDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
