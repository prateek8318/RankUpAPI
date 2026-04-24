using System.Text.Json;
using System.Text.Json.Serialization;
using QuestionService.Application.DTOs;

namespace QuestionService.API.Helpers
{
    public class MockTestStatusConverter : JsonConverter<MockTestStatus>
    {
        public override MockTestStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value switch
            {
                "Active" => MockTestStatus.Active,
                "Inactive" => MockTestStatus.Inactive,
                "Draft" => MockTestStatus.Draft,
                "1" => MockTestStatus.Active,
                "2" => MockTestStatus.Inactive,
                "3" => MockTestStatus.Draft,
                _ => MockTestStatus.Active
            };
        }

        public override void Write(Utf8JsonWriter writer, MockTestStatus value, JsonSerializerOptions options)
        {
            var statusText = value switch
            {
                MockTestStatus.Active => "Active",
                MockTestStatus.Inactive => "Inactive",
                MockTestStatus.Draft => "Draft",
                _ => "Unknown"
            };
            writer.WriteStringValue(statusText);
        }
    }
}
