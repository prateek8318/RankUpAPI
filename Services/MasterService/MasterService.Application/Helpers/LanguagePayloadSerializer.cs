using System.Text.Json;

namespace MasterService.Application.Helpers
{
    internal static class LanguagePayloadSerializer
    {
        public static string? SerializeNames<T>(IEnumerable<T>? items, Func<T, object> selector)
        {
            if (items == null)
            {
                return null;
            }

            var materialized = items.Select(selector).ToList();
            return materialized.Count == 0 ? null : JsonSerializer.Serialize(materialized);
        }

        public static string? SerializeItems<T>(IEnumerable<T>? items, Func<T, object> selector)
        {
            return SerializeNames(items, selector);
        }
    }
}
