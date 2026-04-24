using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
{
    public enum QuestionType
    {
        MCQ = 1,
        TrueFalse = 2,
        FillInBlanks = 3
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public class Question : BaseEntity
    {
        // Question metadata
        public int? ModuleId { get; set; } // FK to Master Service Modules table
        public int ExamId { get; set; } // FK to Master Service Exams table
        public int SubjectId { get; set; } // FK to Master Service Subjects table
        public int? TopicId { get; set; } // FK to Topics table

        // English question content (default)
        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? OptionA { get; set; }

        [MaxLength(500)]
        public string? OptionB { get; set; }

        [MaxLength(500)]
        public string? OptionC { get; set; }

        [MaxLength(500)]
        public string? OptionD { get; set; }

        [Required]
        [MaxLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, D

        public string? Explanation { get; set; }

        // Question properties
        [Required]
        public decimal Marks { get; set; } = 1.00m;

        [Required]
        public decimal NegativeMarks { get; set; } = 0.00m;

        [Required]
        public string DifficultyLevel { get; set; } = "Medium"; // Easy, Medium, Hard

        [Required]
        public string QuestionType { get; set; } = "MCQ"; // MCQ, TrueFalse, FillInBlanks

        // Image URLs
        [MaxLength(500)]
        public string? QuestionImageUrl { get; set; }

        [MaxLength(500)]
        public string? OptionAImageUrl { get; set; }

        [MaxLength(500)]
        public string? OptionBImageUrl { get; set; }

        [MaxLength(500)]
        public string? OptionCImageUrl { get; set; }

        [MaxLength(500)]
        public string? OptionDImageUrl { get; set; }

        [MaxLength(500)]
        public string? ExplanationImageUrl { get; set; }

        // Additional properties
        public bool SameExplanationForAllLanguages { get; set; } = false;

        [MaxLength(500)]
        public string? Reference { get; set; }

        public string? Tags { get; set; } // JSON array of tags

        // Audit fields
        [Required]
        public int CreatedBy { get; set; } // Admin user ID

        public int? ReviewedBy { get; set; } // Reviewer admin user ID

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishDate { get; set; }

        // Navigation properties
        public virtual Topic? Topic { get; set; }
        public virtual ICollection<QuestionTranslation> Translations { get; set; } = new List<QuestionTranslation>();
        
        // Runtime properties for cursor pagination
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int RowNum { get; set; }
    }
}
