namespace ExamService.Domain.Entities
{
    public class ExamQualification : BaseEntity
    {
        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; } = null!;
        
        public int QualificationId { get; set; }
        // Note: Qualification is managed by another service, we only store the ID
    }
}
