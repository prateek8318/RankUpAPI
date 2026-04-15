namespace TestService.Domain.Entities
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal Accuracy { get; set; }
        public int TimeTakenSeconds { get; set; }
    }
}

