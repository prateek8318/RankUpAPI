using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    /// <summary>
    /// Join entity for many-to-many relationship between TestSeries and Question
    /// </summary>
    public class TestSeriesQuestion : BaseEntity
    {
        [Required]
        public int TestSeriesId { get; set; }

        [ForeignKey(nameof(TestSeriesId))]
        public TestSeries TestSeries { get; set; } = null!;

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        // Order of question in the test series
        public int QuestionOrder { get; set; } = 0;

        // Marks for this question in this test series
        public int Marks { get; set; } = 1;
    }
}
