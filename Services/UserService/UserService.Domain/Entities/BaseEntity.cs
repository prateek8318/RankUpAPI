namespace UserService.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Helper method to get India Standard Time
        public DateTime CreatedAtIST => CreatedAt.AddHours(5.5);
    }
}
