using QuestionService.Domain.Entities;

namespace QuestionService.Application.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionTextEnglish { get; set; } = string.Empty;
        public string QuestionTextHindi { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string? QuestionImageUrlEnglish { get; set; }
        public string? QuestionImageUrlHindi { get; set; }
        public string OptionAEnglish { get; set; } = string.Empty;
        public string OptionBEnglish { get; set; } = string.Empty;
        public string OptionCEnglish { get; set; } = string.Empty;
        public string OptionDEnglish { get; set; } = string.Empty;
        public string OptionAHindi { get; set; } = string.Empty;
        public string OptionBHindi { get; set; } = string.Empty;
        public string OptionCHindi { get; set; } = string.Empty;
        public string OptionDHindi { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? ExplanationEnglish { get; set; }
        public string? ExplanationHindi { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int ChapterId { get; set; }
        public int Marks { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateQuestionDto
    {
        public string QuestionTextEnglish { get; set; } = string.Empty;
        public string QuestionTextHindi { get; set; } = string.Empty;
        public QuestionType Type { get; set; } = QuestionType.Text;
        public string OptionAEnglish { get; set; } = string.Empty;
        public string OptionBEnglish { get; set; } = string.Empty;
        public string OptionCEnglish { get; set; } = string.Empty;
        public string OptionDEnglish { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
        public int ChapterId { get; set; }
        public int Marks { get; set; } = 1;
    }

    public class UpdateQuestionDto
    {
        public int Id { get; set; }
        public string QuestionTextEnglish { get; set; } = string.Empty;
        public string QuestionTextHindi { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public int Marks { get; set; }
    }
}
