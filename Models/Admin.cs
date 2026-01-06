using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Admin : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        [Required]
        public string Role { get; set; } = "Admin"; // Can be "SuperAdmin", "Admin", etc.
        
        public bool CanManageUsers { get; set; } = true;
        public bool CanManageContent { get; set; } = true;
    }
}
