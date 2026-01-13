using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Video : BaseEntity
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
        
        public int? ExamId { get; set; }
        public virtual Exam? Exam { get; set; }
        
        public int? SubjectId { get; set; }
        public virtual Subject? Subject { get; set; }
        
        public int? ChapterId { get; set; }
        public virtual Chapter? Chapter { get; set; }
        
        public int Duration { get; set; } // in seconds
        public int ViewCount { get; set; } = 0;
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        public bool IsPremium { get; set; } = false;
    }
}
