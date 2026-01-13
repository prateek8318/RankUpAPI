using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public enum RestrictionType
    {
        Warning = 1,
        Restricted = 2,
        Blocked = 3
    }

    public class UserRestriction : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        public RestrictionType Type { get; set; }
        
        [MaxLength(500)]
        public string? Reason { get; set; }
        
        [MaxLength(1000)]
        public string? Details { get; set; }
        
        public DateTime? RestrictionStartDate { get; set; }
        public DateTime? RestrictionEndDate { get; set; } // null = permanent
        
        public bool IsActive { get; set; } = true;
        
        public int? CreatedByAdminId { get; set; }
        public virtual Admin? CreatedByAdmin { get; set; }
    }
}
