namespace RankUpAPI.Models;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
}
