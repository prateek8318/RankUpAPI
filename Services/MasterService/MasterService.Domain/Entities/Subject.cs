using System;

namespace MasterService.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<SubjectLanguage> SubjectLanguages { get; set; }
    }

    public class SubjectLanguage
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Subject Subject { get; set; }
        public virtual Language Language { get; set; }
    }
}
