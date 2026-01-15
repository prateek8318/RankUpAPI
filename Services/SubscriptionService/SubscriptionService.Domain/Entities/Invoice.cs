using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum InvoiceStatus
    {
        Generated = 1,
        Sent = 2,
        Downloaded = 3,
        Failed = 4
    }

    public class Invoice : BaseEntity
    {
        [Required]
        public int UserSubscriptionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal Subtotal { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        public decimal TaxAmount { get; set; } = 0;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Generated;

        [MaxLength(500)]
        public string? BillingAddress { get; set; }

        [MaxLength(500)]
        public string? CustomerEmail { get; set; }

        [MaxLength(1000)]
        public string? CustomerName { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DownloadedAt { get; set; }

        [MaxLength(500)]
        public string? PdfFilePath { get; set; }

        // Navigation properties
        public virtual UserSubscription UserSubscription { get; set; } = null!;
    }
}
