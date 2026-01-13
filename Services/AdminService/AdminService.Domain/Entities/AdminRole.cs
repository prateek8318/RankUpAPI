namespace AdminService.Domain.Entities
{
    public class AdminRole : BaseEntity
    {
        public int AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;
        
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}
