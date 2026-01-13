using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class Permission : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Resource { get; set; } // e.g., "Users", "Exams", "Payments"
        
        [MaxLength(50)]
        public string? Action { get; set; } // e.g., "Create", "Read", "Update", "Delete"
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
