namespace SharedModels
{
    // JWT Token के लिए shared model
    public class ServiceToken
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = string.Empty; // Bearer, Service, etc.
        public int UserId { get; set; }
        public string UserRole { get; set; } = string.Empty;
    }

    // Inter-service communication के लिए
    public class ServiceRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public object? Body { get; set; }
    }

    // Service authentication response
    public class ServiceAuthResponse
    {
        public bool IsValid { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
