namespace UserService.Application.DTOs
{
    public class UserLanguageDataResponse
    {
        public Dictionary<string, object> States { get; set; } = new();
        public Dictionary<string, object> Qualifications { get; set; } = new();
        public Dictionary<string, object> ExamCategories { get; set; } = new();
        public string CurrentLanguage { get; set; } = "en";
        public bool HasLanguageData { get; set; } = true;
    }

    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class QualificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class ExamCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
