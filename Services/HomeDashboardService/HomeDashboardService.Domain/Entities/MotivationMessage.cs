using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class MotivationMessage : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Author { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public MessageType Type { get; set; } = MessageType.Daily;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsGreeting { get; set; } = false;
    }

    public enum MessageType
    {
        Daily = 1,
        Weekly = 2,
        Achievement = 3,
        Greeting = 4
    }
}
