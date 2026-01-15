using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class DailyVideo : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string VideoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        [MaxLength(100)]
        public string? VideoId { get; set; }

        [Required]
        public DateTime VideoDate { get; set; }

        public int ViewCount { get; set; } = 0;

        [MaxLength(100)]
        public string? PlaylistId { get; set; }
    }
}
