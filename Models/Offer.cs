using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Offer : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? OfferCode { get; set; }
        
        public decimal DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int MaxUses { get; set; } = 0; // 0 = unlimited
        public int CurrentUses { get; set; } = 0;
        
        [MaxLength(50)]
        public string? ApplicableTo { get; set; } // Subscription, MockTest, All
    }
}
