namespace SubscriptionService.Domain.Interfaces
{
    public interface IRazorpayService
    {
        Task<RazorpayOrderResponse> CreateOrderAsync(decimal amount, string currency = "INR", string receipt = null);
        Task<RazorpayOrderDetails> GetOrderDetailsAsync(string orderId);
        Task<bool> VerifyPaymentAsync(string orderId, string paymentId, string signature);
        Task<RazorpaySubscriptionResponse> CreateSubscriptionAsync(PlanSubscriptionRequest request);
        Task<bool> CancelSubscriptionAsync(string subscriptionId);
        Task<RazorpayRefundResponse> ProcessRefundAsync(string paymentId, decimal amount);
        Task<RazorpayPaymentDetails> GetPaymentDetailsAsync(string paymentId);
    }

    public class RazorpayOrderResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Receipt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Attempts { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RazorpayOrderDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Receipt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Attempts { get; set; }
        public List<RazorpayPaymentDetail> Payments { get; set; } = new();
    }

    public class RazorpayPaymentDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PlanSubscriptionRequest
    {
        public int PlanId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public int TotalCount { get; set; } // Number of billing cycles
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }

    public class RazorpaySubscriptionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CurrentStart { get; set; }
        public DateTime CurrentEnd { get; set; }
        public DateTime EndedAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int TotalCount { get; set; }
        public int PaidCount { get; set; }
        public decimal CustomerId { get; set; }
        public string PlanId { get; set; } = string.Empty;
    }

    public class RazorpayRefundResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string RefundId => Id; // Alias for compatibility
    }

    public class RazorpayPaymentDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string InvoiceId { get; set; } = string.Empty;
        public bool International { get; set; }
        public string Method { get; set; } = string.Empty;
        public decimal AmountRefunded { get; set; }
        public decimal RefundStatus { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
