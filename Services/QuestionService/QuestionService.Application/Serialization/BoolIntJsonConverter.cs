using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuestionService.Application.Serialization
{
    /// <summary>
    /// Allows reading booleans from 0/1 (and "0"/"1") values.
    /// Useful when DB projections return numeric flags.
    /// </summary>
    public sealed class BoolIntJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number => reader.TryGetInt64(out var n) ? n != 0 : throw new JsonException("Invalid numeric boolean."),
                JsonTokenType.String => ReadFromString(reader.GetString()),
                _ => throw new JsonException($"Unexpected token {reader.TokenType} for boolean.")
            };
        }

        private static bool ReadFromString(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            if (bool.TryParse(s, out var b))
            {
                return b;
            }

            if (long.TryParse(s, out var n))
            {
                return n != 0;
            }

            throw new JsonException("Invalid string boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}

