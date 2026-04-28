namespace QuestionService.Application.DTOs
{
    public class MockTestGroupedQuestionResponseDto
    {
        public IReadOnlyList<MockTestQuestionGroupDto> MockTestGroups { get; set; } = new List<MockTestQuestionGroupDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class MockTestQuestionGroupDto
    {
        public int MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public int MockTestTypeId { get; set; }
        public string MockTestTypeName { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int QuestionCount { get; set; }
        public IReadOnlyList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}

