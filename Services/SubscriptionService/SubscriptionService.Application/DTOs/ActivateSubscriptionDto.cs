using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Application.DTOs
{
    public class ActivateSubscriptionDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int SubscriptionPlanId { get; set; }
        
        [Required]
        public decimal AmountPaid { get; set; }
        
        [Required]
        public string PaymentId { get; set; } = string.Empty;
        
        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;
        
        public string? RazorpayPaymentId { get; set; }
        
        public string? RazorpaySignature { get; set; }
        
        public bool AutoRenewal { get; set; } = false;
        
        public int? TestsTotal { get; set; }
        
        public DateTime? RenewalDate { get; set; }
    }
}
