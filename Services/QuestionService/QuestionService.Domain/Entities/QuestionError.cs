using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
{
    public class QuestionError : BaseEntity
    {
        [Required]
        public int BatchId { get; set; }

        [Required]
        public int RowNumber { get; set; }

        [Required]
        public string ErrorMessage { get; set; } = string.Empty;

        public string? RawData { get; set; } // The raw Excel/CSV row data

        // Navigation properties
        public virtual QuestionBatch Batch { get; set; } = null!;
    }
}
