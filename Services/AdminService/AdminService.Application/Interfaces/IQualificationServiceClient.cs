using System.Text.Json;

namespace AdminService.Application.Interfaces
{
    public interface IQualificationServiceClient
    {
        Task<object?> CreateQualificationAsync(JsonElement createDto);
    }
}
