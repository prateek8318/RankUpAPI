using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public virtual ICollection<AdminRole> AdminRoles { get; set; } = new List<AdminRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
