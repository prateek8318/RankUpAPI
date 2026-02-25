using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public List<SubjectLanguageDto> SubjectLanguages { get; set; }
    }

    public class CreateSubjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public List<CreateSubjectLanguageDto> SubjectLanguages { get; set; }
    }

    public class UpdateSubjectDto
    {
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool? IsActive { get; set; }
        
        public List<UpdateSubjectLanguageDto> SubjectLanguages { get; set; }
    }

    public class SubjectListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SubjectLanguageDto
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
        public LanguageDto Language { get; set; }
    }

    public class CreateSubjectLanguageDto
    {
        [Required]
        public int LanguageId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSubjectLanguageDto
    {
        public int LanguageId { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
