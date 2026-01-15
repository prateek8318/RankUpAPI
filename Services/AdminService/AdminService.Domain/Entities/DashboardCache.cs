using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class DashboardCache : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string CacheKey { get; set; } = string.Empty; // e.g., "DashboardMetrics_2024-01-15"

        [Required]
        public string CacheData { get; set; } = string.Empty; // JSON string

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(50)]
        public string CacheType { get; set; } = string.Empty; // e.g., "DashboardMetrics", "UserStats"
    }
}
