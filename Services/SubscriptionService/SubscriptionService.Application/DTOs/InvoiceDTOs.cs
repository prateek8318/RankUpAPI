using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int UserSubscriptionId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "INR";
        public InvoiceStatus Status { get; set; }
        public string? BillingAddress { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? DownloadedAt { get; set; }
        public string? PdfFilePath { get; set; }
        public UserSubscriptionDto? UserSubscription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DownloadInvoiceDto
    {
        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class InvoiceDownloadResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; }
        public byte[]? PdfData { get; set; }
        public string? ContentType { get; set; } = "application/pdf";
        public string? FileName { get; set; }
    }
}
