namespace TestService.Domain.Entities
{
    public class UserAvailableTest
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int PracticeModeId { get; set; }
        public int? SubjectId { get; set; }
        public int? SeriesId { get; set; }
        public int? Year { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsLocked { get; set; }
        public bool IsUnlocked { get; set; }
        public int? SubscriptionPlanId { get; set; }
        public string? SubscriptionPlanName { get; set; }
    }
}
