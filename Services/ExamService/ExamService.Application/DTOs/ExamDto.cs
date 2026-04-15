namespace ExamService.Application.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }
        public List<int> QualificationIds { get; set; } = new();
        public List<int?> StreamIds { get; set; } = new();

        public int? ExamCategoryId { get; set; }
        public int? ExamTypeId { get; set; }
        public int? SubjectId { get; set; }

        public int TotalQuestions { get; set; }
        public decimal MarksPerQuestion { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }

        public string AccessType { get; set; } = "Free";
        public int? SubscriptionPlanId { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? ValidTill { get; set; }

        public int AttemptsAllowed { get; set; }
        public string ShowResultType { get; set; } = "Immediately";
        public string Status { get; set; } = "Draft";
        
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; } = false;
        public List<int>? QualificationIds { get; set; }
        public List<int?>? StreamIds { get; set; }

        public int? ExamCategoryId { get; set; }
        public int? ExamTypeId { get; set; }
        public int? SubjectId { get; set; }

        public int TotalQuestions { get; set; }
        public decimal MarksPerQuestion { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }

        public string AccessType { get; set; } = "Free";
        public int? SubscriptionPlanId { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? ValidTill { get; set; }

        public int AttemptsAllowed { get; set; } = 1;
        public string ShowResultType { get; set; } = "Immediately";
        public string Status { get; set; } = "Draft";

        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
    }

    public class UpdateExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }
        public List<int>? QualificationIds { get; set; }
        public List<int?>? StreamIds { get; set; }

        public int? ExamCategoryId { get; set; }
        public int? ExamTypeId { get; set; }
        public int? SubjectId { get; set; }

        public int TotalQuestions { get; set; }
        public decimal MarksPerQuestion { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }

        public string AccessType { get; set; } = "Free";
        public int? SubscriptionPlanId { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? ValidTill { get; set; }

        public int AttemptsAllowed { get; set; }
        public string ShowResultType { get; set; } = "Immediately";
        public string Status { get; set; } = "Draft";

        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
    }

    public class QualificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
