using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class AdminPaymentListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
        
        public int? UserId { get; set; }
        
        public string? TransactionId { get; set; }
        
        public PaymentStatus? Status { get; set; }
        
        public PaymentMethod? PaymentMethod { get; set; }
        
        public decimal? AmountFrom { get; set; }
        
        public decimal? AmountTo { get; set; }
        
        public DateTime? CreatedDateFrom { get; set; }
        
        public DateTime? CreatedDateTo { get; set; }
        
        public string? ProviderOrderId { get; set; }
    }
}
