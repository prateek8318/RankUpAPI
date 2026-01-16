using System.Text.Json;

namespace AdminService.Application.Interfaces
{
    public interface IMasterServiceClient
    {
        Task<object?> CreateLanguageAsync(JsonElement createDto);
        Task<object?> CreateStateAsync(JsonElement createDto);
    }
}
