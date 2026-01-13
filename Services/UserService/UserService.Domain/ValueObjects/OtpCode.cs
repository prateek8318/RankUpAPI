namespace UserService.Domain.ValueObjects
{
    public class OtpCode
    {
        public string Code { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public string PhoneNumber { get; private set; }

        public OtpCode(string code, string phoneNumber, int expiryMinutes = 5)
        {
            Code = code;
            PhoneNumber = phoneNumber;
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
        }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsValid(string code) => Code == code && !IsExpired;
    }
}
