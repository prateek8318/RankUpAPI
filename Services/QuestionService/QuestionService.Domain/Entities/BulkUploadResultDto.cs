namespace QuestionService.Domain.Entities
{
    public class BulkUploadResultDto
    {
        public int BatchId { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
