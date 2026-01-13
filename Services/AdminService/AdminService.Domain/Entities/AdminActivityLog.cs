using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class AdminActivityLog : BaseEntity
    {
        public int AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;
        
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // e.g., "CreateExam", "UpdateUser"
        
        [MaxLength(200)]
        public string? Resource { get; set; } // e.g., "Exam", "User"
        
        public int? ResourceId { get; set; }
        
        [MaxLength(1000)]
        public string? Details { get; set; }
        
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
}
