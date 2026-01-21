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
        public List<StateLanguageDto> Names { get; set; } = new();
    }

    public class StateLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreateStateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
        public List<StateLanguageCreateDto>? Names { get; set; }
    }

    public class StateLanguageCreateDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateStateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
        public List<StateLanguageCreateDto>? Names { get; set; }
    }
}
