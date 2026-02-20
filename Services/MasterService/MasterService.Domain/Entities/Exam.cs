using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Exam : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional country for which this exam is primarily relevant.
        /// Links to Country.Code.
        /// </summary>
        [MaxLength(10)]
        public string? CountryCode { get; set; }

        public Country? Country { get; set; }

        /// <summary>
        /// Minimum recommended age of candidate.
        /// </summary>
        public int? MinAge { get; set; }

        /// <summary>
        /// Maximum recommended age of candidate.
        /// </summary>
        public int? MaxAge { get; set; }

        /// <summary>
        /// Optional image representing the exam (logo/banner).
        /// Only URL is stored here; upload handled by other services.
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Indicates if this is an international exam.
        /// </summary>
        public bool IsInternational { get; set; } = false;

        public ICollection<ExamLanguage> ExamLanguages { get; set; } = new List<ExamLanguage>();

        /// <summary>
        /// Mapping to qualifications/streams this exam applies to.
        /// </summary>
        public ICollection<ExamQualification> ExamQualifications { get; set; } = new List<ExamQualification>();
    }
}

