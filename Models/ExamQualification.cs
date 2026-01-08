using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    public class ExamQualification
    {
        public int ExamId { get; set; }
        public int QualificationId { get; set; }

        // Navigation properties
        public Exam Exam { get; set; } = null!;
        public Qualification Qualification { get; set; } = null!;
    }
}
