using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Threading.Tasks;
using UserService.Application.DTOs;

namespace UserService.API.Binders
{
    public class NoValidationModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var json = reader.ReadToEndAsync().Result;
            request.Body.Position = 0;

            // Fix: Case-insensitive deserialization
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var model = JsonSerializer.Deserialize<MinimalSocialLoginRequestDto>(json, options);
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}