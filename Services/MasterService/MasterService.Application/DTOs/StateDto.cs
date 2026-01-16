namespace MasterService.Application.DTOs
{
    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
    }

    public class UpdateStateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
    }
}
