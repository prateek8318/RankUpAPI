using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string? PlanType { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool AutoRenew { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public int UserId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string? PlanType { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; } = false;
    }

    public class UserManagementStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSubscribers { get; set; }
        public int FreeUsers { get; set; }
        public int NewUsersLast7Days { get; set; }
        public int BlockedUsers { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class UserManagementDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "Active"; // Active, Blocked, Inactive
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool HasActiveSubscription { get; set; }
        public string? PlanName { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }
        public int? SubscriptionId { get; set; }
        public decimal? SubscriptionAmount { get; set; }
        public int DaysRemaining { get; set; }
    }

    public class BlockUserDto
    {
        public int UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool Block { get; set; } = true; // true = block, false = unblock
    }

    public class BlockUserResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
