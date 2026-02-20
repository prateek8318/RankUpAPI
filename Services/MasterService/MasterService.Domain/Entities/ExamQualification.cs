namespace MasterService.Domain.Entities
{
    public class ExamQualification : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public int QualificationId { get; set; }
        public Qualification Qualification { get; set; } = null!;

        /// <summary>
        /// Optional stream within the qualification.
        /// </summary>
        public int? StreamId { get; set; }
        public Stream? Stream { get; set; }
    }
}

