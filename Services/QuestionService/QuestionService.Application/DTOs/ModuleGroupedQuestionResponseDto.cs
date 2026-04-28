namespace QuestionService.Application.DTOs
{
    public class ModuleGroupedQuestionResponseDto
    {
        public IReadOnlyList<ModuleQuestionGroupDto> ModuleGroups { get; set; } = new List<ModuleQuestionGroupDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class ModuleQuestionGroupDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public IReadOnlyList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public IReadOnlyList<SubjectQuestionGroupDto> Subjects { get; set; } = new List<SubjectQuestionGroupDto>();
    }

    public class SubjectQuestionGroupDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public IReadOnlyList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
